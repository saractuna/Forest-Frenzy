using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjectsBehaviour : MonoBehaviour
{
    [SerializeField] GameObject [] gameObjectsPrefabs;
    [SerializeField] GameObject spotIndicator;
    [SerializeField] GameObject target;
    [SerializeField] Vector3 spawnHeight;
    [SerializeField] float spawnTimeInterval = 10f;
    

    private float minSpawnDistance = -0.5f;
    private float maxSpawnDistance = 0.5f;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            // MonoBehvaiour Class that basically Invokes a Function/Method Repeatatively 
            InvokeRepeating("SpawnObject", 2f, spawnTimeInterval);
            InvokeRepeating("SpawnObject", 15f, 15f);
        }
        catch(System.Exception ex)
        {
            Debug.LogError("Error Message in Falling Object Beh Script" + ex.Message);
        }      
    }

    // Function to spawn GameObject Prefabs
    void SpawnObject()
    {
        float randomspawnDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        // A new Vector 3 Variable for the Spawn Position of the Game Objects, Random.Range is a random nunmber generator from the specfified range of number given. 
        Vector3 spawnPosition = new Vector3(target.transform.position.x + randomspawnDistance, target.transform.position.y + spawnHeight.y, target.transform.position.z  + Random.Range(-0.5f, 0.5f)); // The Falling Object would spawn based on the player postion + the random spawn distance
        // Debug.Log($"Y pos for spawn position is {randomspawnDistance}");

        Vector3 indicatorPosition = new Vector3(spawnPosition.x, spawnPosition.y - spawnPosition.y + 1.3f, spawnPosition.z); // This is the position of the indicator that would be spawned at the same position as the falling objects but the Y axis would be on the ground level
        GameObject indicator = Instantiate(spotIndicator, indicatorPosition , Quaternion.identity); // Spawns the spot indicator at the random spawn position
         
         
        Destroy(indicator, 4f); // Destroys the indicator after the specified time
         

        // This Variable gets a random Index from the Array and what ever prefab that index is attached to that gameobject would be instantiated
        int randomArrayIndex = Random.Range(0, gameObjectsPrefabs.Length);
        GameObject fallingObject = Instantiate(gameObjectsPrefabs[randomArrayIndex],spawnPosition, Quaternion.identity); // Quaternion.identity, basically gives the game object no quaternion rotation / Instantiate is a function that essential spawns a gameobject into the scene
        rb = fallingObject.GetComponent<Rigidbody>();
        rb.angularVelocity = Vector3.up * 1f;  
    }   
}
