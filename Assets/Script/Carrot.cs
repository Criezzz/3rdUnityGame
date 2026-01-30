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
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
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
        // Check if hit enemy
        if (other.CompareTag("Enemy"))
        {
            // Try to deal damage to regular enemy
            // For now, just destroy the enemy
            Debug.Log($"Carrot hit enemy: {other.gameObject.name}");
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Boss"))
        {
            // Deal damage to boss
            BossHealth bossHealth = other.GetComponent<BossHealth>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damage);
            }
            Debug.Log($"Carrot hit boss for {damage} damage!");
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            // Hit ground/wall
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Backup collision handling if trigger doesn't work
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Boss"))
        {
            BossHealth bossHealth = collision.gameObject.GetComponent<BossHealth>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
