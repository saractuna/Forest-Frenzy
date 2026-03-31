using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeehiveMovement : MonoBehaviour
{
    public float moveSpeed = 2f;

    private float movementDirection = 1f;

    public float boundaryLeft = -10f;

    public float boundaryRight = 10f;

    void Update()
    {
        if (transform.position.x < boundaryLeft)
        {
            movementDirection = 1f;
        }
        else if (transform.position.x > boundaryRight) 
        {
            movementDirection = -1f;
        }

        transform.position += new Vector3(moveSpeed * movementDirection * Time.deltaTime, 0, 0);
    }
}
