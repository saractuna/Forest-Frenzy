using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set;} // Other CS can access this but can't change anything from this Script Manager
    
    #region // Variables
    // Player Variables 
    GameObject player;
    public int playerLives = 0; 
    public int intLives = 4;

    // Managers

    ScoreTableManager scoreTableManager;
    BeehiveMovement beehiveMovementScript;

    PlayerController playerControllerScript;

    

    // For Beehive 
    [SerializeField] GameObject[] beehivePrefabs;
    [SerializeField] float speedMultiplier = 2f; // Increase the speed of the beehive for diffiuclty
    [SerializeField] int maxNumberOfBeehives;
    [SerializeField] public List<GameObject> activeBeehives = new List<GameObject>();
    [SerializeField] int currentNumberOfBeehives = 0;


    // General Game Variables
    public float timerleft;
    public bool timerOn = false;
    public int score = 0;
    public int totalscore = 0;
    public int round = 0;
    public int level = 1;
    bool levelTransitioning = false;


    // For Audio
    AudioManager audioManager;


    // For Ui
    UIManager uIManager;


    // Gamestates
    public enum GameState {playing, death, gameOver, paused}  // Enum for the Gamestates
    public GameState currentGameState;
    #endregion

    private void Awake()
    {
        if(Instance != null && Instance != this) // If there is an instance of this script and it is not this script, then destroy this script
        {
            Destroy(this);
        }
        else
        {
            Instance = this; // If there is no instance of this script, then this script is the instance
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript =  GameObject.Find("Player").GetComponent<PlayerController>();
        player = GameObject.Find("Player"); // Finds the GameObject in the Hierarchy for the name Player
        uIManager = GameObject.Find("HUD Canvas").GetComponent<UIManager>(); // Finds the GameObject for the UI Canvas and gets the UIManager Script
        scoreTableManager = GameObject.Find("ScoreTable Manager").GetComponent<ScoreTableManager>();
        SetGameState(GameState.playing);
        ResetGame(); // Resets the Game at start of this script

        SpawnBeehives(); // To start the game with a beehive
        currentNumberOfBeehives = level; // Tracking number of beehives
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputs();
        Timer();
        DebugKeys();
    }

    void Timer()
    {
        if (timerOn) // Timer is on at the start of the script 
        {
            if(timerleft > 0 ) // If the timer is greater than 0, then the timer will decrease by 1 second
            {
                timerleft -=  Time.deltaTime;  
            }
            else  // If the timer is less than 0, then the timer will be 0 and the timer will be off
            {
                timerleft = 0;
                timerOn = false;
                SetGameState(GameState.gameOver);
                
                
            }
              
        }
    }

    public void AddPoints(int value) // Adds Points when a Target is hit
    {
        score += value;
        uIManager.UpdateGameUiHudElements(); // Updates the UI for the Points gained
    }

    public void ResetGame() // Resets the game when the player loses the game 
    {
        playerLives = intLives;
        score = 0;
        round = 0;
        timerleft = 60;
        timerOn = true;
        uIManager.UpdateGameUiHudElements();
        uIManager.ResetUI();       
    }

    // Gamestates 
    public void SetGameState(GameState newGameState) // Method to set the Gamestate, uses a switch statement for each gamestate and invokes other methods and this method takes input so that we can change the gamestate
    {
        currentGameState = newGameState;
        switch (currentGameState)
        {
            case GameState.playing:
                PlayingState();
                break;
            case GameState.death:
                DeathState();
                break;
            case GameState.gameOver:
                GameOver();
                break;
            case GameState.paused:
                PausedState();
                break;
        }
    }  

    void PlayingState() // Ui for Playing State and put time to 1, which plays the game
    {
        uIManager.deathScreen.SetActive(false);
        uIManager.gameOverScreen.SetActive(false);
        uIManager.pausedScreen.SetActive(false);
        Time.timeScale = 1;
        AudioManager.Instance.PlayMusic(AudioManager.AudioType.GameMusic);
        Debug.Log("Game is in Play State");
        
    }

    void DeathState() // Ui for Death State (when the player loses all their lives) and put time to 0, which pauses the game
    {
        uIManager.deathScreen.SetActive(true);
        scoreTableManager.HighScoreTableDataUpdate();
        Time.timeScale = 0;
        AudioManager.Instance.PlayMusic(AudioManager.AudioType.GameMusic);
        Debug.Log("Game is in Death State");
    }

    void GameOver() // Ui for Game Over when the timer finishes
    {
        uIManager.gameOverScreen.SetActive(true);
        scoreTableManager.HighScoreTableDataUpdate();
        Time.timeScale = 0;
        AudioManager.Instance.PlayMusic(AudioManager.AudioType.GameMusic);
        Debug.Log("Game is in Game Over State");
    }

    void PausedState() // Ui for Paused State and put time to 0, which pauses the game 
    {
        uIManager.pausedScreen.SetActive(true);
        Time.timeScale = 0;
        Debug.Log("Game is in Paused State");
        AudioManager.Instance.StopMusic();
       
    }

    

     void CheckInputs()
     {
        if(Input.GetButtonDown("Cancel")) // If the Escape button is pressed
        {
            Debug.Log("Escape Button Pressed");
            if(currentGameState == GameState.playing) // And if the gamestate is at playing, then pause the game
            {
                SetGameState(GameState.paused);
            }
            else if(currentGameState == GameState.paused) // if the gamestate is paused, then play the game
            {
                SetGameState(GameState.playing);
            }
        }
        if(currentGameState == GameState.playing) // Only allow the High Score Menu When the current game state is playing
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                scoreTableManager.DisplayCurrentHighScoreData();
                uIManager.highScoreTableMenu.SetActive(true);
            }
            else if(Input.GetKeyUp(KeyCode.Tab))
            {
                uIManager.highScoreTableMenu.SetActive(false);
            }
        }
        
     }

     void DifficultyIncrease()
     { 
        float randomSpeed = Random.Range(1f, 3f); 
        beehiveMovementScript.moveSpeed += randomSpeed;
        float randomTime = Random.Range(5f, 20f);
        timerleft += randomTime; // Adds additional Timer
        uIManager.DisplayAdditionalTimeAdded(randomTime);
       
     }

     void SpawnBeehives()
     {
        // Spawns beehives EQUAL to the current level number
        for (int i = 0; i < level; i++)
        {
            // Values for the spawn coordinates
            float randomSpawnPosition = Random.Range(-10f, 10f); // x
            float randomSpawnDistance = Random.Range(3f, 6f); // z (distance from the player's side)
            float randomSpawnHeight = Random.Range(1f, 2f); // y 
            
            Vector3 spawnPosition = new Vector3(transform.position.x + randomSpawnPosition, transform.position.y + randomSpawnHeight, transform.position.z - randomSpawnDistance); // Random spawn position for the beehives
            GameObject beehive = Instantiate(beehivePrefabs[Random.Range(0, beehivePrefabs.Length)], spawnPosition, Quaternion.identity);
            beehiveMovementScript = beehive.transform.Find("Beehive").GetComponent<BeehiveMovement>(); // Gets the child Gameobject, which is the beehive from the instantiated Beehive Holder and gets the Beehive Movement script 
            activeBeehives.Add(beehive);
            
        }
     }

    public void BeehiveDestroyed()
     {
        AudioManager.Instance.PlaySound(AudioManager.AudioType.BoomerangHit);
        currentNumberOfBeehives--;
        AddPoints(1);
        uIManager.UpdateGameUiHudElements();

        if (currentNumberOfBeehives <= 0)
        {
            NextLevel();
        }
    }

    public void NextLevel()
    {
        AudioManager.Instance.PlaySound(AudioManager.AudioType.LevelUp);
        level++;
        uIManager.UpdateGameUiHudElements();

        SpawnBeehives(); // Spawn the new set of beehives
        currentNumberOfBeehives = level; // Keeps track of the number of beehives that exist

        levelTransitioning = false;

       switch(level)
       {
            case 2:
            DifficultyIncrease();
            break;
            case 3:
            uIManager.addTimeText.gameObject.SetActive(false); // to turn off the text 
            break;
            case 4:
            DifficultyIncrease();
            break;
            case 5:
            uIManager.addTimeText.gameObject.SetActive(false);
            break;
            case 6:
            DifficultyIncrease();
            break;
            case 7:
            uIManager.addTimeText.gameObject.SetActive(false);
            break;
            case 8:
            DifficultyIncrease();
            break;
            case 9:
            uIManager.addTimeText.gameObject.SetActive(false);
            break;
            case 10:
            DifficultyIncrease();
            break;
            case 11:
            uIManager.addTimeText.gameObject.SetActive(false);
            break;
            case 12:
            DifficultyIncrease();
            break;
            case 13:
            uIManager.addTimeText.gameObject.SetActive(false);
            break;
            case 14:
            DifficultyIncrease();
            break;
            case 15:
            uIManager.addTimeText.gameObject.SetActive(false);
            break;
            case 16:
            DifficultyIncrease();
            break;
            case 17:
            uIManager.addTimeText.gameObject.SetActive(false);
            break;
            case 18:
            DifficultyIncrease();
            break;
            case 19:
            uIManager.addTimeText.gameObject.SetActive(false);
            break;
            case 20:
            DifficultyIncrease();
            break;
            case 21:
            uIManager.addTimeText.gameObject.SetActive(false);
            break;
            case 22:    
            DifficultyIncrease();
            break;
            case 23:
            uIManager.addTimeText.gameObject.SetActive(false);
            break;
       }
    }

     public void PlayerTakesDamage() // If the player takes damage, then the player lives will decrease by 1
     {
        AudioManager.Instance.PlaySound(AudioManager.AudioType.PlayerHit);
        playerLives--;
        uIManager.TakeLivesUI();

         if(playerLives <= 0) // If the player lives are less than 0, then the gamestate will be in the death state / Game Over!
         {
             SetGameState(GameState.death);
             scoreTableManager.HighScoreTableDataUpdate();
         }
     }
     
     void DebugKeys()
     {
        if(currentGameState == GameState.playing) // So that the Debug Keys only work when the game state is in Play mode
        {
            if(Input.GetKeyDown(KeyCode.F1)) // Debug Key for taking Lives
            {
                PlayerTakesDamage();
                
            }
            if(Input.GetKeyDown(KeyCode.F2)) // Debug Key for adding additional lives 
            {
                playerLives++; 
                uIManager.AddLivesUI(); 
            }
            if(Input.GetKeyDown(KeyCode.T)) // Stop The Timer
            {
                if(timerOn) // If the timer is on then turn it off
                {
                    timerOn = false;
                }
                else
                {
                    timerOn = true;
                }
            }
            if(Input.GetKeyDown(KeyCode.F3)) // Subtracts 10 seconds from the timer 
            {
                timerleft -= 10f;
            }
            if(Input.GetKeyDown(KeyCode.F4)) // ADDS 10 Seconds to the timer
            {
                timerleft += 10f;
            }
           
            if(Input.GetKeyDown(KeyCode.F5)) // Debug Key for Increase the Level
            {
                if(activeBeehives != null) // If there are any active beehives, then
                {
                    foreach (GameObject beehive in activeBeehives) // for each beehives 
                    {
                        Destroy(beehive); // Destroy them 
                    }
                    activeBeehives.Clear();  // Clears everything in the list 
                }
                NextLevel(); // Spawns the next level of beehives
            }
            
            if(Input.GetKeyDown(KeyCode.F6) && scoreTableManager._delMessageGameObject.activeInHierarchy == false) // Debug key for deleting the saved high score data
            {
                scoreTableManager.DeleteSavedHighScoreData();
                scoreTableManager._delMessageGameObject.SetActive(true);
            }
            else if(Input.GetKeyDown(KeyCode.F6) && scoreTableManager._delMessageGameObject.activeInHierarchy == true)
            {
                scoreTableManager._delMessageGameObject.SetActive(false);
            }

            if(Input.GetKeyDown(KeyCode.G) && uIManager.debugKeyText.gameObject.activeInHierarchy == false) // Disp[lay the Debug Key Text
            {
                uIManager.debugKeyText.gameObject.SetActive(true);
            }
            else if(Input.GetKeyDown(KeyCode.G) && uIManager.debugKeyText.gameObject.activeInHierarchy == true)
            {
                uIManager.debugKeyText.gameObject.SetActive(false);
            }
            
        }        
    }
}
