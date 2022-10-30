using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementController2D : MonoBehaviour
{
    // Movement
    [SerializeField] float maxSpeed = 7f;
    [SerializeField] float jumpMaxHeigth = 5f;
    [SerializeField] float acceleration = 40f;
    [SerializeField] float deceleration = 25f;

    Vector3 velocity = Vector3.zero;

    void OnGUI()
    {
        GUI.skin.box.fontSize = 20;

        GUI.Box(new Rect(3, 3, 200, 200), "Stats\n\n");
    }

    void Update()
    {
        float directionInput = GetPlayerInput();

        Move(directionInput);
    }

    float GetPlayerInput()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    void Move(float directionInput)
    {

        // If player is moving speed him up ...
        if (directionInput != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, directionInput * maxSpeed, acceleration * Time.deltaTime);
        }
        // ... else slow him down
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, deceleration * Time.deltaTime);
        }

        transform.Translate(velocity * Time.deltaTime);
    }

    // Jumping
    void Jump()
    {

    }
}
