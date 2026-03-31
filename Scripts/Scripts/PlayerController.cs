using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    #region
    // For Player Movement 
    public float moveSpeed = 3.5f;
    [SerializeField] float rotationSpeed = 50f; // Change to make roation speed faster or slower

    Vector2 move;
    Vector3 targetRotation; // Was spelled wrong :/
    Rigidbody rb;

    // For Boomerang Mechanics
    [SerializeField] private GameObject boomerangPrefab;
    [SerializeField] private Transform boomerangSpawnPoint;
    
    [SerializeField] private float minThrowDistance = 5f;
    [SerializeField] private float maxThrowDistance = 20f;
    [SerializeField] private float maxHoldTime = 1.15f;

    private bool isHoldingThrow = false;
    private float throwHoldStartTime = 0f;
    
    // For Animations
    Animator playerAnimator;
    [SerializeField] bool isWalking = false;
    [SerializeField] bool isIdle = false;
    [SerializeField] bool isAttacking = false;
    public  bool isTakingDamage = false;

    private ParticleSystem[] footStepParticles;
    #endregion
   
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        footStepParticles = GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        PlayerMovement();     
        CheckInputForBoomerang();
        SetAnimations();
        PlayFootStepEffects();
    }

  
   void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>(); // Gets the values from the keys pressed from the input system
    }

    void CheckInputForBoomerang()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isAttacking = true;
            isHoldingThrow = true;
            throwHoldStartTime = Time.time; // Stores the time when button is first pressed
        }
        else if (Input.GetMouseButtonUp(0) && isHoldingThrow)
        {
            isAttacking = false;
            isHoldingThrow = false;

            // Calculates the time of the button held
            float holdDuration = Mathf.Clamp(Time.time - throwHoldStartTime, 0f, maxHoldTime);

            // Normalizes the hold time to a value between 0 and 1 (0% and 100%)
            float throwStrength = holdDuration / maxHoldTime;

            // Calculates the throw distance
            float throwDistance = Mathf.Lerp(minThrowDistance, maxThrowDistance, throwStrength);

            ThrowBoomerang(throwDistance);
        }
    }

    void PlayerMovement()
    {
        // movement varaible is a new vector 3, getting in the values from the input system
        Vector3 movement = new Vector3(move.x, 0, move.y);
       

        if (movement == Vector3.zero)
        {
            // Makes it look forward when done moving. Special thanks to Andrei!
            rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.05f);


            isWalking = false;
            isIdle = true;
            return;
        }
        else 
        {
            targetRotation = Quaternion.LookRotation(movement).eulerAngles; // Target rotation is the rotation of the player relative to the movement, which is the input from the keys

        }
        
        // This changes the player rotation according to what where ever we're moving, Slerp interpolates between two rotations 
        rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRotation.x, Mathf.Round(targetRotation.y / 45 * 45), targetRotation.z), Time.deltaTime * rotationSpeed);  // Changed the Roatation of the player so that it considers 8 directional movement
       

        // This moves the player's transform position
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        // Sprinting Mechanic
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            moveSpeed = 5.5f;
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = 3.5f;
        }
        

        // Just had to remove the IF statements to fix auto walking issue :d
        isWalking = true;
        isIdle = false;
    }

    void ThrowBoomerang(float throwDistance)
    {
        // Checks for a currently existing boomerang in the scene
        if (GameObject.FindGameObjectWithTag("Boomerang") != null)
        {
            // If a boomerang exists, does nothing
            return;
        }

        if (boomerangPrefab != null && boomerangSpawnPoint != null)
        {
            // Shoots a raycast from the camera to the mouse pointer
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //To ingore barriers so boomerang doesn't misbehave
            int barrierLayer = LayerMask.NameToLayer("Barrier");
            int layerMask = ~(1 << barrierLayer); // Idk what this does but it has to be here

            Vector3 targetPosition;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) // Apply layer mask
            {
                if (hit.collider.CompareTag("Player"))
                {
                    // If the raycast hits the player, ignore the hit position and just throw the damn boomerang forward!
                    targetPosition = boomerangSpawnPoint.position + transform.forward * throwDistance;
                }
                else
                {
                    // If the raycast hits somewhere, assign the mouse position as the target position for the throw
                    targetPosition = hit.point;
                }
            }
            else
            {
                // If there is no hit, just aim in the direction on the Z plane (with the latest throwDistance calculation)
                targetPosition = ray.GetPoint(throwDistance);
                targetPosition.y = boomerangSpawnPoint.position.y;
            }

            // Determines the throws direction
            Vector3 throwDirection = (targetPosition - boomerangSpawnPoint.position).normalized;

            // Spawns the boomerang from the assigned prefab
            GameObject boomerang = Instantiate(boomerangPrefab, boomerangSpawnPoint.position, Quaternion.identity);
            Boomerang boomerangScript = boomerang.GetComponent<Boomerang>();

            if (boomerangScript != null)
            {
                boomerangScript.Throw(boomerangSpawnPoint, throwDirection, throwDistance); // Send throw distance
            }
        }
    }

    void SetAnimations()
   {
        playerAnimator.SetBool("walk", isWalking);
        playerAnimator.SetBool("idle", isIdle);
        playerAnimator.SetBool("attack", isAttacking);
        playerAnimator.SetBool("damage", isTakingDamage);
   }

   void PlayFootStepEffects()
   {
     if(isWalking)
     {
        //AudioManager.Instance.PlaySound(AudioManager.AudioType.Walking);
        foreach (ParticleSystem footStepParticle in footStepParticles)
        {
            footStepParticle.Play();    
        }
     }
     else if(isIdle)
     {
        //AudioManager.Instance.StopSound(AudioManager.AudioType.Walking);    
        foreach (ParticleSystem footStepParticle in footStepParticles)
        {
            footStepParticle.Stop();
        }
     }
   }
}