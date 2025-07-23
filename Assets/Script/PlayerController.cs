using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction playerMovement;
    public InputAction playerJump;
    public InputAction attack;
    public Rigidbody2D rg;
    public Animator animator;
    public float speed;
    public float fallSpeed;
    public bool onGround;
    public bool moving;
    public bool lookingRight;
    public bool lookingLeft;
    public AnimatorOverrideController rightController;
    public AnimatorOverrideController leftController;
    public bool stunned;
    [SerializeField] public float jumpHeight;
    //[SerializeField] public float jumpSpeed;
    private void Awake()
    {
        jumpHeight = 23f;
        onGround = true;
        moving = false;
        lookingRight = true;
        lookingLeft = false;
        stunned = false;
        fallSpeed = 8f;
        speed = 9f;
    }
    private void OnEnable()
    {
        playerMovement.Enable();
        attack.Enable();
        playerJump.performed += jump;
        playerJump.canceled += cancelJump;
        playerJump.Enable();
    }
    private void OnDisable()
    {
        playerMovement.Disable();
        playerJump.Disable();
        attack.Disable();

        playerJump.performed -= jump;
        playerJump.canceled -= cancelJump;
    }
    void Start()
    {
      animator = GetComponent<Animator>();
    }
    void Update()
    {
        
        
    }
    void FixedUpdate()
    {
        if (lookingRight) {
            animator.runtimeAnimatorController = rightController;
        }
        else {
            animator.runtimeAnimatorController = leftController;
        }
        int direction = playerMovement.ReadValue<Vector2>().x > 0 ? 1 : playerMovement.ReadValue<Vector2>().x < 0 ? -1 : 0;
        if (direction != 0)
        {
            moving = true;
            if (direction == 1)
            {
                lookingRight = true;
                lookingLeft = false;
            }
            else
            {
                lookingRight = false;
                lookingLeft = true;
            }
        }
        else
        {
            moving = false;
        }
        rg.linearVelocity = new Vector2(playerMovement.ReadValue<Vector2>().x * speed, rg.linearVelocity.y);

        
       
        //slow down the jump
        if (rg.linearVelocity.y < -Mathf.Abs(fallSpeed))
        {
            rg.linearVelocity = new Vector2(rg.linearVelocity.x, Mathf.Clamp(rg.linearVelocity.y, -Mathf.Abs(fallSpeed), Mathf.Infinity));
                
        }
          
    }
    void jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump action performed");
        if (onGround)
        {
            Debug.Log("Jumping");
            rg.linearVelocity = new Vector2(rg.linearVelocity.x, jumpHeight);

         

        }
    }
    void cancelJump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump action canceled");
        if (rg.linearVelocity.y > 0)
        {
            rg.linearVelocity = new Vector2(rg.linearVelocity.x, 0);

        }
    }

}
