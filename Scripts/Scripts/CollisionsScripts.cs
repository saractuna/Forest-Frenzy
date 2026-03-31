using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionsScripts : MonoBehaviour
{

    // Manager & Scripts

    LevelManager levelManager;
    PowerUpsSpawner powerUpsSpawner;
    
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            levelManager = GameObject.Find("Level Manager").GetComponent<LevelManager>();
            powerUpsSpawner = GameObject.Find("PowerUp Spawner Holder").GetComponent<PowerUpsSpawner>();
        }
        catch(System.Exception ex)
        {
            Debug.LogError("Error in the Collision Script: " + ex.Message);
        }
        
    }
    void OnCollisionEnter(Collision other) // Collision Check for Falling Objects
    {
        GameObject gameObjectHit = other.gameObject; // Gets reference to the GameObject that is hit
        if (gameObjectHit.CompareTag("Player")) // If this GameObject has a tag Player then player takes damage and then destroy the game object immediately
        {
            //Debug.Log($"Falling Object has hit the {gameObjectHit}");
            levelManager.PlayerTakesDamage();
            Destroy(gameObject);
        }       
        Destroy(gameObject, 1f); // Destroy the Falling GameObject after 5 Seconds if it collides with something else other than the player      
    }

    void OnTriggerEnter(Collider other) // Collision Check for PowerUp
    {
        GameObject gameObjectHit = other.gameObject;

        if(gameObjectHit.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySound(AudioManager.AudioType.PowerUpCollected);
            //Debug.Log(gameObject.tag);
            switch (gameObject.tag)
            {
                case "Health":
                powerUpsSpawner._HealthPowerUpCollected = true;
                powerUpsSpawner.HealthPowerUp();
                Debug.Log("Health collected");
                break;
                case "Speed":
                Debug.Log("speed collected");
                powerUpsSpawner._SpeedPowerUpCollected = true;
                break;
            }
            
           // Debug.Log("Player has received " + gameObject.name);
            Destroy(powerUpsSpawner.spawnedPowerUp);
        }
    }
}
