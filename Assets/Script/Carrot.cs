using UnityEngine;

public class Carrot : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 15f;
    public float damage = 10f;
    public float lifetime = 3f;
    
    private Vector2 direction;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Ensure collider is a trigger so we pass through and detect hits
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        
        // Configure rigidbody for kinematic movement (we control velocity directly)
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    void Start()
    {
        // Destroy after lifetime
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
        
        // Rotate sprite to face direction
        // Sprite is vertical (pointing up), so we need to adjust:
        // - Shooting right: rotate -90° (point right)
        // - Shooting left: rotate 90° (point left)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
        else
        {
            transform.Translate(direction * speed * Time.fixedDeltaTime, Space.World);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.gameObject);
    }

    private void HandleHit(GameObject other)
    {
        // Check if hit enemy (by tag or layer)
        if (other.CompareTag("Enemy") || (other.layer == 8 && !other.CompareTag("Boss")))
        {
            Debug.Log($"Carrot hit enemy: {other.name}");
            Destroy(other);
            Destroy(gameObject);
            return;
        }
        
        // Check if hit boss (by tag, layer, or parent tag)
        if (IsBoss(other))
        {
            // Try to find BossHealth on this object or parent
            BossHealth bossHealth = other.GetComponent<BossHealth>();
            if (bossHealth == null)
            {
                bossHealth = other.GetComponentInParent<BossHealth>();
            }
            
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damage);
                Debug.Log($"Carrot hit boss for {damage} damage!");
            }
            else
            {
                Debug.LogWarning($"Carrot hit boss object but no BossHealth found: {other.name}");
            }
            Destroy(gameObject);
            return;
        }
        
        // Check if hit ground
        if (other.CompareTag("Ground") || other.layer == 3)  // Layer 3 is often Ground
        {
            Destroy(gameObject);
            return;
        }
    }

    private bool IsBoss(GameObject obj)
    {
        // Check tag directly
        if (obj.CompareTag("Boss"))
            return true;
        
        // Check parent tag
        if (obj.transform.parent != null && obj.transform.parent.CompareTag("Boss"))
            return true;
        
        // Check if on Enemy layer (8) with Boss-related name
        if (obj.layer == 8 && (obj.name.Contains("Boss") || obj.name.Contains("Body")))
            return true;
        
        // Check root object
        if (obj.transform.root.CompareTag("Boss"))
            return true;
        
        return false;
    }
}
