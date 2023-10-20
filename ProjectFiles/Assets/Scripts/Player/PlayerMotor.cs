using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    public bool isGrounded;
    
    public float speed = 5.0f;
    //private float CurrentSpeed;
    public float sprintSpeed = 8f;
    public float gravity = -9.8f;
    public float jumpForce = 0.5f;
    //public float drag = 3f;
    public float groundResistance = 10f;
    public float airResistance = 5f;
    public bool EnableMidAirControl = false;
    private Vector3 DirectionCache;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
    }
    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = new Vector3(input.x, 0f, input.y);
        float resistance = isGrounded ? groundResistance : airResistance;
        if(isGrounded || EnableMidAirControl)
        {
            DirectionCache.x = Mathf.MoveTowards(DirectionCache.x, moveDirection.x, resistance * Time.deltaTime);
            DirectionCache.z = Mathf.MoveTowards(DirectionCache.z, moveDirection.z, resistance * Time.deltaTime);
        }

        Vector3 currentDirection = transform.TransformDirection(DirectionCache);
        Vector3 move = currentDirection * speed * Time.deltaTime;

        playerVelocity.y += gravity * Time.deltaTime;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        controller.Move(move + playerVelocity * Time.deltaTime);

        if (controller.collisionFlags == CollisionFlags.Above)
        {
            playerVelocity.y = 0f;
        }
    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpForce * -3.0f * gravity);
        }
    }
    public void Sprint(bool IsSprinting)
    {
        if (IsSprinting)
            speed = sprintSpeed;
        else
            speed = 5f;
    }
}
