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

    public float speed;
    public bool onGround;
    [SerializeField] public float jumpHeight;
    private void Awake()
    {
        jumpHeight = 22f;
        onGround = true;
        speed = 9f;
    }
    private void OnEnable()
    {
        playerMovement.Enable();
        playerJump.Enable();
        attack.Enable();
        playerJump.performed += jump;
        playerJump.canceled += cancelJump;
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
      
    }
    void Update()
    {
        
        
    }
    void FixedUpdate()
    {
        if(onGround)
        {
           rg.linearVelocity = new Vector2(playerMovement.ReadValue<Vector2>().x * speed, rg.linearVelocity.y);

        }
        else
        {
            
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
