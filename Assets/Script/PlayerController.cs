using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction playerMovement;
    public InputAction playerJump;
    public InputAction attack;
    public Rigidbody2D rg;

    public float speed = 7f;
    public bool onGround = true;
    [SerializeField] public float jumpHeight = 2f;
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
        rg.velocity = new Vector2(playerMovement.ReadValue<Vector2>().x * speed, rg.velocity.y);
    }
    void jump(InputAction.CallbackContext context)
    {
        if (onGround)
        {
            rg.velocity = new Vector2(rg.velocity.x, jumpHeight);
            onGround = false;
        }
    }
    void cancelJump(InputAction.CallbackContext context)
    {
        if (rg.velocity.y > 0)
        {
            rg.velocity = new Vector2(rg.velocity.x, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            onGround = true;
        }
    }

}
