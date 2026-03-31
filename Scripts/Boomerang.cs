using System.Collections;
using UnityEngine;

public class Boomerang : MonoBehaviour
{

    // Level Manager 
    LevelManager levelManager;
    private Transform returnPoint; // Returning point of the boomerang (holder)
    private Vector3 throwDirection; // Direction of the thrown boomerang
    private bool returning = false;

    [SerializeField] private float throwSpeed = 10f;
    [SerializeField] private float returnSpeed = 15f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private float waveHeight = 0.5f;
    [SerializeField] private float waveFrequency = 2f;
    private ParticleSystem beehiveImpactParticles;

    private Vector3 startPosition;
    private float throwStartTime;

    void Start()
    {

       try
       {
            levelManager = GameObject.Find("Level Manager").GetComponent<LevelManager>();
       }
       catch(System.Exception ex)
       {
            Debug.LogError("Error in Boomerang Script" + ex.Message);
       }
            
        
        
    }

    public void Throw(Transform boomerangSpawnPoint, Vector3 direction, float throwDistance)
    {
        returnPoint = boomerangSpawnPoint; // Sets the return point to the spawn point fixing the flooring problem
        throwDirection = direction.normalized; // Normalizes direction for speed to be consistent (no diagonals)
        startPosition = transform.position;
        throwStartTime = Time.time;
        maxDistance = throwDistance;
        AudioManager.Instance.PlaySound(AudioManager.AudioType.BoomerangThrow, 1f, 1f);
        AudioManager.Instance.PlaySound(AudioManager.AudioType.BoomerangSpin, 0.5f, 1f);
        StartCoroutine(BoomerangBehavior());
    }

    private IEnumerator BoomerangBehavior()
    {
        while (!returning)
        {
            float waveOffset = Mathf.Sin((Time.time - throwStartTime) * waveFrequency) * waveHeight; // Uses sine wave to mimic a wave motion :)
            Vector3 newPosition = transform.position + throwDirection * throwSpeed * Time.deltaTime;
            newPosition.y += waveOffset * Time.deltaTime;

            // Rotates the boomerang to add spin
            transform.position = newPosition;
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            // Checks for distance then starts the return phase if true
            if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
            {
                returning = true;
            }
            yield return null;
        }

        while (returning)
        {
            transform.position = Vector3.MoveTowards(transform.position, returnPoint.position, returnSpeed * Time.deltaTime);
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            // Destroys the boomerang if it has returned
            if (Vector3.Distance(transform.position, returnPoint.position) < 0.5f)
            {
                Destroy(gameObject);
                AudioManager.Instance.StopSound(AudioManager.AudioType.BoomerangSpin);
            }
            yield return null;
        }
        

    }

    void OnTriggerEnter(Collider collider) // OnTrigger is used so that the boomerang doesnt affect physics related things
    {
        GameObject gameObjectHit = collider.gameObject; // Gets the reference to the GameObject that the boomerang hits
        //Debug.Log(gameObjectHit.ToString()); // Debug Log to check what the boomerang hit
        if(gameObjectHit.CompareTag("Beehive")) // if the gameobject that the boomerang hits is the Beehive then Destroy the beehive and returning is set to true 
       {
            beehiveImpactParticles = gameObjectHit.transform.Find("Impact Particle Effect").GetComponent<ParticleSystem>();
            beehiveImpactParticles.Play();
            
            //Debug.Log("Hit the beehive");
            levelManager.activeBeehives.Remove(gameObjectHit.transform.parent.gameObject);
            levelManager.BeehiveDestroyed();
            Destroy(gameObjectHit.transform.parent.gameObject, 0.11f);
            returning = true;
       }
      
       
    }
}