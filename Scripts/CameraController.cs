using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform playerTarget;
    [SerializeField] Vector3 offset;
    [SerializeField] float smoothTime = 0.3f;
    private Transform currentTarget;
    private Vector3 velocity = Vector3.zero; // Vector3.Zero is a shorthand for writing the vectors (0,0,0)
   
    void Start()
    {
        currentTarget = playerTarget; // Start by following the player
        
    }

    void Update()
    {
        // Check for a boomerang in the scene
        GameObject boomerang = GameObject.FindWithTag("Boomerang");

        // Switches camera to boomerang if there is an object with the tag 'Boomerang' in scene, if not, follows the player.
        if (boomerang != null)
        {
            currentTarget = boomerang.transform;
        }
        else
        {
            currentTarget = playerTarget;
        }

        // If there is a target, smoothly follow it
        if (currentTarget != null)
        {
            // Offset to adjust position of the camera according to the target object (player/boomerang)
            Vector3 targetPos = currentTarget.position + offset;
            
            // SmoothDamp basically gradually changes the vectors of the camera postion to the player's position
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
 
            // Nice Polishing implementation of a camera movement mechanic   
            transform.LookAt(currentTarget); // Look at the current target which changes from player to boomerang

        }

        if (currentTarget != playerTarget) // So if the Current Target the Boomerang
        {
            Camera.main.fieldOfView = 80f; // Zooms out from the boomerang to give a better view
            
        }
        else
        {
            Camera.main.fieldOfView = 60f; // Resets back to normal FOV
        }
             
    }
}