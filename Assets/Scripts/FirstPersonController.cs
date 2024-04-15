using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mixpanel;
using UnityEngine.SceneManagement;

public class FirstPersonController : MonoBehaviour
{
    public CharacterController controller;

    public enum MoveState
    {
        Walk,
        Sprint,
        Crouch,
        InAir
    }
    
    [Header("Gravity")] 
    public float gravity;
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;
    public bool isGrounded;

    [Header("Movement")] 
    public MoveState moveState;
    public float walkSpeed = 12f;
    public float crouchSpeed = 10f;
    public float crouchScale;
    private float startScale;
    public float sprintSpeed = 15f;
    private float gunStartScale;
    private float gunStartY;
    private float gunCrouchY = -0.5f;
    public GameObject gun;
    
    private float speed;
    private Vector3 velocity;
    private Vector3 move;
    public float jumpHeight;

    public HeatmapData playerHeatMap;
    private float heatMapTimer = 4.0f;


    void Start()
    {
        startScale = transform.localScale.y;
        gunStartScale = gun.transform.localScale.y;
        gunStartY = gun.transform.localPosition.y;
        playerHeatMap = new HeatmapData();
        playerHeatMap.levelName = SceneManager.GetActiveScene().name;
    }
    
    // Update is called once per frame
    void Update()
    {
        if(GameManager.isGamePaused)
        {
            return;
        }

        // calculate updates
        GravityUpdate();
        Movement();
        Jump();
        Crouch();
        CheckState();
        
        // apply updates to player controller
        controller.Move(move * speed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);
        
        // analytics: add player position data every 8 seconds
        if (!FirstPersonManager.isFpsPaused)
        {
            heatMapTimer -= Time.deltaTime;
            if (heatMapTimer <= 0.0f)
            {
                Debug.Log("heat map: LOGGING PLAYER POSITION");
                heatMapTimer = 4.0f;
                playerHeatMap.playerPosition.Add(transform.position);
            }
        }
    }

    void CheckState()
    {
        if (isGrounded && (Input.GetKey(KeyCode.LeftShift)))
        { 
            // SPRINTING (hold)
            moveState = MoveState.Sprint;
            speed = sprintSpeed;
        }
        else if (isGrounded && Input.GetKeyDown(KeyCode.C))
        {
            // CROUCHING (hold)
            moveState = MoveState.Crouch;
            speed = crouchSpeed;
        }
        else if (isGrounded)
        {
            // WALKING
            moveState = MoveState.Walk;
            speed = walkSpeed;
        }
        else
        { 
            // IN AIR?? FROM JUMPING???
            moveState = MoveState.InAir;
        }
    }

    void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        move = transform.right * x + transform.forward * z;
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void Crouch()
    {
        //Debug.Log("called crouch() function");
        if (Input.GetKeyDown(KeyCode.C))
        {
            //Debug.Log("entered crouch");
            transform.localScale = new Vector3(transform.localScale.x, crouchScale, transform.localScale.z);
            transform.position -= Vector3.down * startScale * crouchScale;
            gun.transform.localScale = new Vector3(gun.transform.localScale.x, gunStartScale / crouchScale, gun.transform.localScale.z);
            gun.transform.localPosition =
                new Vector3(gun.transform.localPosition.x, gunCrouchY, gun.transform.localPosition.z);
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            transform.localScale = new Vector3(transform.localScale.x, startScale, transform.localScale.z);
            gun.transform.localScale = new Vector3(gun.transform.localScale.x, gunStartScale, gun.transform.localScale.z);
            gun.transform.localPosition =
                new Vector3(gun.transform.localPosition.x, gunStartY, gun.transform.localPosition.z);
        }
        
    }

    void GravityUpdate()
    {
        // gravity
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        velocity.y += gravity * Time.deltaTime;
    }
}
