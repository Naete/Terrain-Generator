using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float xCoordinateSpeed = 0;
    [SerializeField] private float yCoordinateSpeed = 0;
    [SerializeField] private float groundSpeed = 0;
    [SerializeField] private int maxWalkSpeed = 6;
    
    private const float Acceleration = 0.046875f;
    private const float Deceleration = 0.5f;
    private const float Friction = 0.046875f;

    void Update()
    {
        Move();

        transform.Translate(new Vector3(groundSpeed, 0, 0) * Time.deltaTime);
    }

    void Move()
    {
        float direction = Input.GetAxisRaw("Horizontal");

        // Pressing left
        if (direction == -1)
        {
            if (groundSpeed > 0)
            {
                groundSpeed -= Deceleration;

                if (groundSpeed <= 0)
                {
                    groundSpeed = -0.5f;
                }
            }
            else if (groundSpeed > -maxWalkSpeed)
            {
                groundSpeed -= Acceleration;

                if (groundSpeed <= -maxWalkSpeed)
                {
                    groundSpeed = -maxWalkSpeed;
                }
            }
        }

        // Pressing right
        if (direction == 1)
        {
            if (groundSpeed < 0)
            {
                groundSpeed += Deceleration;

                if (groundSpeed >= 0)
                {
                    groundSpeed = 0.5f;
                }
            }
            else if (groundSpeed < maxWalkSpeed)
            {
                groundSpeed += Acceleration;

                if (groundSpeed >= maxWalkSpeed)
                {
                    groundSpeed = maxWalkSpeed;
                }
            }
        }

        if (direction == 0)
            groundSpeed -= Mathf.Min(Mathf.Abs(groundSpeed), Friction) * Mathf.Sign(groundSpeed);
    }
}
