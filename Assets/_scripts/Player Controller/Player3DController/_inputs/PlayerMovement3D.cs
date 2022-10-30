using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
    public Camera cam;

    private PlayerInputs inputActions;
    private Vector2 movementInput;
    public float moveSpeed = 10f;
    private Vector3 inputDirection;
    private Vector3 moveVector;
    private Quaternion currentRotation;

    private void Awake() {
        inputActions = new PlayerInputs();
        inputActions.Player.Movement.performed += context => movementInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate() {
        float h = movementInput.x;
        float v = movementInput.y;

        Vector3 targetInput = new Vector3(h, 0, v);

        inputDirection = Vector3.Lerp(inputDirection, targetInput, Time.deltaTime * 10f);

        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;

        Vector3 desiredDirection = camForward * inputDirection.z + camRight * inputDirection.x;

        Move(desiredDirection);
        Turn(desiredDirection);
    }

    void Move(Vector3 desiredDir) {
        moveVector.Set(desiredDir.x, 0f, desiredDir.z);
        moveVector = moveVector * moveSpeed * Time.deltaTime;
        transform.position += moveVector;
    }

    void Turn(Vector3 desiredDir) {
        if ((desiredDir.x > 0.1 || desiredDir.x < -0.1) || (desiredDir.z > 0.1 || desiredDir.z < -0.1)) {
            currentRotation = Quaternion.LookRotation(desiredDir);
            transform.rotation = currentRotation;
        } else {
            transform.rotation = currentRotation;
        }
    }

    private void OnEnable() {
        inputActions.Enable();
    }

    private void OnDisable() {
        inputActions.Disable();
    }
}
