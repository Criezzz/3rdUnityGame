using UnityEngine;

public class BossController1 : MonoBehaviour
{
    public Animator anim;
    private float jumpCooldown = 5f;
    private float jumpTimer = 0f;
    public Rigidbody2D rb;
    void Start() {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        jumpTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.T))
        {
            anim.SetTrigger("bossPreJump");
            Debug.Log("Jump");
        }
        if (jumpTimer <= 0f)
        {
            if (Random.value < 0.4f) 
            {
                anim.SetTrigger("bossPreJump");

            }
            jumpTimer = jumpCooldown;
        }
    }


}