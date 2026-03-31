using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;


public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pointsText;
    [SerializeField] TextMeshProUGUI timer;
    [SerializeField] TextMeshProUGUI Level;
    public TextMeshProUGUI addTimeText;
    public TextMeshProUGUI debugKeyText;
    [SerializeField] List<GameObject> currentActiveHeartPrefabs;

    [SerializeField] GameObject heartprefab;
    [SerializeField] Transform uiHeartsHolder;

    // Level Manager
    LevelManager levelManager;
   
    public GameObject deathScreen, gameOverScreen, pausedScreen, highScoreTableMenu; 

   

    // For Timer
    int intTimer;

    // Start is called before the first frame update
    void Start()
    {
        levelManager =  GameObject.Find("Level Manager").GetComponent<LevelManager>();
        for(int i = 0; i < levelManager.intLives; i++)
        {
            GameObject _InstantiatedHearts = Instantiate(heartprefab, uiHeartsHolder);
            currentActiveHeartPrefabs.Add(_InstantiatedHearts);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
      UpdateGameUiHudElements();
    }

    public void LoadDifferentLevel(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

   

    public void UpdateGameUiHudElements()
    {
        intTimer = (int)LevelManager.Instance.timerleft; // Cast the data type from float to int for better ui display

        pointsText.text = "Score:"  + LevelManager.Instance.score.ToString(); // Displays the Score
        timer.text = "Time Remaining:"  + intTimer; // Displays the Timer
        Level.text = "Level: "  + LevelManager.Instance.level.ToString(); // Displays the Level
        
    }

    public void ResetUI()
    {
        deathScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        pausedScreen.SetActive(false);
        UpdateGameUiHudElements();
    }

    public void TakeLivesUI() // For when the player takes damage and the Lives Ui needs to be Updated 
    {
        if(currentActiveHeartPrefabs != null)
        {
            int randomArrayIndexInPlayerLivesGameObject = Random.Range(0, currentActiveHeartPrefabs.Count); // Gets a random Index in the List
            Destroy(currentActiveHeartPrefabs[randomArrayIndexInPlayerLivesGameObject]); // Destroys the GameObject in the list
            currentActiveHeartPrefabs.RemoveAt(randomArrayIndexInPlayerLivesGameObject); // Removes the destroyed gameobject from the list

        }
 
    }

    public void AddLivesUI()
    {
        
        GameObject _heartPrefab =  Instantiate(heartprefab, uiHeartsHolder); // Instantiates the Lives Prefab into the Ui
        currentActiveHeartPrefabs.Add(_heartPrefab); // Adds The instantiated heartprefab into the List 
    }

    public void DisplayAdditionalTimeAdded(float randomTime)
    {
        addTimeText.text = "+ " + (int)randomTime +  " Seconds";
        addTimeText.gameObject.SetActive(true);
        

    }
       
}
