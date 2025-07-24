using Unity.VisualScripting;
using UnityEngine;

public class Boss1_Jumping : StateMachineBehaviour
{
    public float jumpHeight = 2f; // How high the boss jumps
    public float jumpDuration = 0.5f; // How long the jump lasts

    private Vector3 startPos;
    private float timer;
    public GameManager gameManager;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        startPos = animator.transform.position;
        timer = 0f;
        gameManager = GameManager.Instance;
        animator.SetBool("isIdle", true);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(timer / jumpDuration);
        // Parabolic jump: up then down
        float yOffset = 4 * jumpHeight * normalizedTime * (1 - normalizedTime);
        animator.transform.position = startPos + new Vector3(0, yOffset, 0);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.position = startPos;
        animator.ResetTrigger("bossJump");
        if (Random.value < 0.5f)
        {
            gameManager.SpawnGroundSlime();
        }
        else
        {
            gameManager.SpawnFlyingSlime();
        }
    }
}