using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{

    public RecoilScript weaponRecoil;
    public CharacterController controller;

    public float speed = 4f;
     public float sprintMultiplier = 1.5f;
    public float crouchSpeed = 6f;
    public float scopedSpeed = 4f;

    public AudioSource footstepAudio; // Reference to the AudioSource
public AudioClip footstepClip; // Reference to the footstep audio clip


    public float gravity = -9.81f;
    public float jumpHeight = 3f; 

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask; 

    private Vector3 velocity;
    public bool isGrounded; 

     private bool isCrouching = false;
    private float originalHeight;
    public float crouchedHeight = 0.5f;

    public float headBobAmount = 0.1f;
    public float headBobSpeed = 5f;
    private float originalY;

    public bool isScoped = false; // To check if the player is scoped

    // Update is called once per frame
      void Start()
    {
        // Optionally find the WeaponRecoil component if it's on the same GameObject
        if (weaponRecoil == null)
        {
            weaponRecoil = GetComponent<RecoilScript>();
        }
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);



        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

         // Handle crouching
        if (Input.GetKeyDown(KeyCode.C))
        {
           ToggleCrouch();
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
          float currentSpeed = isCrouching ? crouchSpeed : (Input.GetKey(KeyCode.LeftShift) ? speed * sprintMultiplier : speed);


           // If scoped, reduce speed
        if (isScoped)
        {
            currentSpeed = scopedSpeed;
        }


        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
         // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

         if (Input.GetButtonDown("Fire1")) // Change to your fire input
    {
        // Assuming you have a reference to the WeaponRecoil script
        weaponRecoil.Fire();
        // Also add shooting logic here
    }

      
    }

      private void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        controller.height = isCrouching ? crouchedHeight : originalHeight;
    }


     // Method to toggle scoped mode
    public void ToggleScope()
    {
        isScoped = !isScoped;
    }
}
