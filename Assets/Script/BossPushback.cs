using UnityEngine;

/// <summary>
/// Pushes player away when they land on top of the boss.
/// Attach to Boss.
/// </summary>
public class BossPushback : MonoBehaviour
{
    public float pushForce = 10f;

    void OnCollisionStay2D(Collision2D collision)
    {
        // Find player rigidbody (could be Player tag or Body child)
        Rigidbody2D playerRb = null;
        Transform playerTransform = null;

        if (collision.gameObject.CompareTag("Player"))
        {
            playerRb = collision.rigidbody;
            playerTransform = collision.transform;
        }
        else if (collision.transform.root.CompareTag("Player"))
        {
            playerRb = collision.transform.root.GetComponent<Rigidbody2D>();
            playerTransform = collision.transform.root;
        }

        if (playerRb == null || playerTransform == null) return;

        // Check if player is above the boss
        if (playerTransform.position.y > transform.position.y + 0.3f)
        {
            float dirX = playerTransform.position.x > transform.position.x ? 1f : -1f;
            playerRb.linearVelocity = new Vector2(dirX * pushForce, pushForce * 0.3f);
        }
    }
}

