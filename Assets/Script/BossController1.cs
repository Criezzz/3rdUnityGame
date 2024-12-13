using UnityEngine;

public class BossController1 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Animator anim;

    // Update is called once per frame
    private void Awake()
    {
        Invoke("jump",5f);
        Invoke("idle", 8f);

    }
    void Update()
    {
    }
    void jump()
    {
        anim.SetBool("isJumping", true);
        anim.SetBool("isIdle", false);
    }
    void idle()
    {
        anim.SetBool("isJumping", false);
        anim.SetBool("isIdle", true);
    }
}
