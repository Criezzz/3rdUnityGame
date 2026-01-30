using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    public PlayerController player;
    public float knockbackForce = 12f;
    public float knockbackUpForce = 6f; // Additional upward force to prevent getting stuck
    public float invincibilityDuration = 1.5f;
    private bool isInvincible = false;
    private Collider2D bodyCollider;

    void Awake()
    {
        player = transform.parent.GetComponent<PlayerController>();
        bodyCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        
    }

    public bool IsInvincible => isInvincible;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject, collision.transform.position);
    }

    // Handle continuous collision to prevent getting stuck
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isInvincible) return;
        
        // If still colliding with enemy during invincibility, push player away
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss"))
        {
            Vector2 pushDirection = (transform.position - collision.transform.position).normalized;
            player.rg.AddForce(pushDirection * 2f, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        HandleCollision(collider.gameObject, collider.transform.position);
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (!isInvincible) return;
        
        if (collider.gameObject.CompareTag("Enemy") || collider.gameObject.CompareTag("Boss"))
        {
            Vector2 pushDirection = (transform.position - collider.transform.position).normalized;
            player.rg.AddForce(pushDirection * 2f, ForceMode2D.Impulse);
        }
    }

    private void HandleCollision(GameObject other, Vector3 otherPosition)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            if (!isInvincible)
            {
                ApplyDamageAndKnockback(otherPosition);
            }
        }
    }

    private void ApplyDamageAndKnockback(Vector3 enemyPosition)
    {
        // Calculate knockback direction (away from enemy)
        Vector2 knockbackDirection = ((Vector2)transform.position - (Vector2)enemyPosition).normalized;
        
        // Always add some upward force to prevent getting stuck under enemies
        // If enemy is above, push down and away; if below or same level, push up and away
        float verticalDiff = transform.position.y - enemyPosition.y;
        
        Vector2 finalKnockback;
        if (verticalDiff < -0.3f) // Enemy is above player
        {
            // Push player down and away horizontally
            finalKnockback = new Vector2(knockbackDirection.x * knockbackForce, -knockbackUpForce * 0.5f);
        }
        else
        {
            // Push player up and away
            finalKnockback = new Vector2(knockbackDirection.x * knockbackForce, Mathf.Max(knockbackUpForce, knockbackDirection.y * knockbackForce));
        }
        
        // Apply knockback
        player.rg.linearVelocity = Vector2.zero;
        player.rg.AddForce(finalKnockback, ForceMode2D.Impulse);
        
        // Notify player of damage
        player.SendMessage("OnDamaged", 1, SendMessageOptions.DontRequireReceiver);
        
        // Start invincibility
        StartCoroutine(InvincibilityCoroutine());
        
        Debug.Log($"Player hit! Knockback: {finalKnockback}");
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        
        // Temporarily ignore collision with Enemy layer (layer 8)
        int playerLayer = transform.parent.gameObject.layer;
        int enemyLayer = 8; // Enemy layer
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        
        // Also ignore for body's layer
        Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayer, true);
        
        // Visual feedback - blink effect
        SpriteRenderer parentSprite = transform.parent.GetComponent<SpriteRenderer>();
        if (parentSprite != null)
        {
            float elapsed = 0f;
            while (elapsed < invincibilityDuration)
            {
                parentSprite.enabled = !parentSprite.enabled;
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.1f;
            }
            parentSprite.enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(invincibilityDuration);
        }
        
        // Re-enable collision with Enemy layer
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayer, false);
        
        isInvincible = false;
    }
}
