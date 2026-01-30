using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    public PlayerController player;
    public float knockbackForce = 12f;
    public float knockbackUpForce = 6f; // Additional upward force to prevent getting stuck
    public float invincibilityDuration = 1.5f;
    public float stompBounceForce = 10f; // Force to bounce player after stomping enemy
    public float stompThreshold = 0.3f; // How far above enemy the player must be to count as stomp
    private bool isInvincible = false;
    private Collider2D bodyCollider;
    private float lastDamageTime = -10f;  // Cooldown to prevent double damage
    private const float DAMAGE_COOLDOWN = 0.5f;  // 500ms cooldown - prevents double hits from multiple colliders

    void Awake()
    {
        // Try to get PlayerController from parent
        player = transform.parent?.GetComponent<PlayerController>();
        bodyCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        // Fallback: try to find player if parent reference failed
        if (player == null)
        {
            player = GetComponentInParent<PlayerController>();
        }
        if (player == null)
        {
            player = transform.root.GetComponent<PlayerController>();
        }
        if (player == null)
        {
            Debug.LogError("Body: Could not find PlayerController!");
        }
    }

    public bool IsInvincible => isInvincible;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject, collision.transform.position);
    }

    // Handle continuous collision - deal damage if not invincible, push away during invincibility
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (player == null) return;
        
        // If not invincible, handle collision as normal (will deal damage)
        if (!isInvincible)
        {
            HandleCollision(collision.gameObject, collision.transform.position);
        }
        else
        {
            // During invincibility, push player away from enemy
            if (IsEnemy(collision.gameObject))
            {
                Vector2 pushDirection = (transform.position - collision.transform.position).normalized;
                player.rg.AddForce(pushDirection * 2f, ForceMode2D.Impulse);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        HandleCollision(collider.gameObject, collider.transform.position);
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (player == null) return;
        
        // If not invincible, handle collision as normal (will deal damage)
        if (!isInvincible)
        {
            HandleCollision(collider.gameObject, collider.transform.position);
        }
        else
        {
            // During invincibility, push player away from enemy
            if (IsEnemy(collider.gameObject))
            {
                Vector2 pushDirection = (transform.position - collider.transform.position).normalized;
                player.rg.AddForce(pushDirection * 2f, ForceMode2D.Impulse);
            }
        }
    }

    /// <summary>
    /// Check if the object is an enemy (Enemy tag or on Enemy layer, but NOT Boss)
    /// Boss is handled separately for instant death
    /// </summary>
    private bool IsEnemy(GameObject obj)
    {
        // Exclude boss - handled separately for instant death
        if (obj.CompareTag("Boss"))
            return false;
        if (obj.transform.parent != null && obj.transform.parent.CompareTag("Boss"))
            return false;
        
        // Check Enemy tag
        if (obj.CompareTag("Enemy"))
            return true;
        
        // Check parent for Enemy
        if (obj.transform.parent != null && obj.transform.parent.CompareTag("Enemy"))
            return true;
        
        // Check if on Enemy layer (layer 8)
        if (obj.layer == 8)
            return true;
        
        return false;
    }

    private void HandleCollision(GameObject other, Vector3 otherPosition)
    {
        // Boss touch = instant death (bypasses invincibility and cooldown)
        if (other.CompareTag("Boss") || (other.transform.parent != null && other.transform.parent.CompareTag("Boss")))
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null && !health.IsDead)
            {
                health.OnDamaged(health.currentHealth); // Deal max damage to kill instantly
            }
            return;
        }

        // Prevent double damage with time-based cooldown
        if (Time.time - lastDamageTime < DAMAGE_COOLDOWN) return;
        
        if (IsEnemy(other))
        {
            // Check for stomp: player is above enemy (even slightly)
            float verticalDiff = transform.position.y - otherPosition.y;
            
            if (verticalDiff > stompThreshold)
            {
                // Stomp successful - kill the enemy!
                StompEnemy(other);
                return;
            }
            
            // Normal collision (from side or below) - take damage
            if (!isInvincible)
            {
                lastDamageTime = Time.time;
                ApplyDamageAndKnockback(otherPosition);
            }
        }
    }

    /// <summary>
    /// Kill enemy by stomping and bounce the player
    /// </summary>
    private void StompEnemy(GameObject enemy)
    {
        // Find the root enemy object to destroy
        GameObject enemyRoot = enemy;
        
        // Check if this is a child of an enemy (like a collider child)
        if (enemy.transform.parent != null && 
            (enemy.transform.parent.CompareTag("Enemy") || enemy.transform.parent.gameObject.layer == 8))
        {
            enemyRoot = enemy.transform.parent.gameObject;
        }
        
        // Bounce player up
        player.rg.linearVelocity = new Vector2(player.rg.linearVelocity.x, 0);
        player.rg.AddForce(Vector2.up * stompBounceForce, ForceMode2D.Impulse);
        
        // Destroy the enemy
        Destroy(enemyRoot);
        
        Debug.Log($"Stomped enemy: {enemyRoot.name}");
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
        
        // Notify player of damage - call PlayerHealth directly to avoid double damage
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.OnDamaged(1);
        }
        
        // Start invincibility
        StartCoroutine(InvincibilityCoroutine());
        
        Debug.Log($"Player hit! Knockback: {finalKnockback}");
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        
        // Visual feedback - blink effect
        SpriteRenderer parentSprite = transform.parent?.GetComponent<SpriteRenderer>();
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
        
        isInvincible = false;
    }
}
