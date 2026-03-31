using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class PowerUpsSpawner : MonoBehaviour
{
    // Managers
    LevelManager levelManager;
    UIManager uIManager;

    PlayerController playerController;

    // Timer

     [SerializeField] float _TempTimer = 5f;

     public bool _SpeedPowerUpCollected = false;
     public bool _HealthPowerUpCollected = false;

    

    [SerializeField] GameObject[] powerUpsPrefabs;
    [SerializeField] GameObject targetSpawnPos;

    [SerializeField] GameObject healthUi;
    [SerializeField] GameObject speedUi;
    
    [SerializeField] private float minSpawnDistance;
    [SerializeField] private float maxSpawnDistance;

    [SerializeField] float spawnTimeIntervals = 15f;
    private float spawnTimerLeft = 5f;
    public GameObject spawnedPowerUp;
    
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            playerController = GameObject.Find("Player").GetComponent<PlayerController>();
            levelManager = GameObject.Find("Level Manager").GetComponent<LevelManager>();
            uIManager  = GameObject.Find("HUD Canvas").GetComponent<UIManager>();
            InvokeRepeating("SpawnPowerUps", 10f, spawnTimeIntervals); // Method is called 10 second after the script is activated and then spawns at different intervals after that
        }
        catch(System.Exception ex)
        {
            Debug.LogError("Error in PowerUpSpawner Script, Error Message : " + ex.Message);
        }
    }

    void Update()
    {
        if(_HealthPowerUpCollected == true) // When Health Power up is collected start the timer from 0
        {
              _TempTimer -= Time.deltaTime; // Timer
              healthUi.SetActive(true); // Set the Ui active if the Health power up is collected 

              if(_TempTimer <= 0)
              {
                healthUi.SetActive(false); // Set it false after the timer is 0
                _HealthPowerUpCollected = false; // Set the Health Power Up Collected to false
                _TempTimer = 5f; // Reset the timer back to 5 seconds
              }

           
        }
       SpeedPowerUp();
    }

    void SpawnPowerUps()
    {
        float randomSpawnDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        int randomIndex = Random.Range(0, powerUpsPrefabs.Length); // Gets a random index in the array of gameobjects this is done so that random powerups spawn
        Vector3 spawnPos = new Vector3(targetSpawnPos.transform.position.x + randomSpawnDistance, targetSpawnPos.transform.position.y + 1f , targetSpawnPos.transform.position.z + Random.Range(-1.0f, -2.5f)); // Spawn position is relative to the transform poisition of the tatget pos 
        spawnedPowerUp = Instantiate(powerUpsPrefabs[randomIndex], spawnPos, Quaternion.identity); 
       
        

        Destroy(spawnedPowerUp, spawnTimerLeft); // Destroys the Powerup spawned after a specified time
        
    }

    public void HealthPowerUp()
    {
        levelManager.playerLives++;
        uIManager.AddLivesUI();
    }

    public void SpeedPowerUp() // This is check in update 
    {
        if(_SpeedPowerUpCollected != false) // This is true when the player collects the speed power up
        {
            if(_TempTimer > 0) // Timer starts at 5 Seconds, so the player's speed would change to 8f for 5 seconds and
            {
                _TempTimer -= Time.deltaTime;
                playerController.moveSpeed = 8f;
                speedUi.SetActive(true);
            }
            else 
            {
                playerController.moveSpeed = 3.5f; // Change the speed back to the defualt speed of the player
                _SpeedPowerUpCollected = false; // Make the Collected Speed Power Up to false since the timer is finished   
                _TempTimer = 5f; // Resets the timer back to 5 seconds
                speedUi.SetActive(false);
            }
       }       
    }
}
