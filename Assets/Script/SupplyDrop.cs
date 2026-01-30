using UnityEngine;

public class SupplyDrop : MonoBehaviour
{
    [Header("Pickup Settings")]
    public int ammoAmount = 3;  // Full refill by default
    public float lifetime = 15f;  // How long it stays on the ground before disappearing
    
    [Header("Visual Effects")]
    public float bobSpeed = 3f;
    public float bobAmount = 0.15f;
    
    private bool hasLanded = false;
    private Vector3 landedPosition;
    private float bobTimer;
    private float groundTimer;

    void Start()
    {
        // Destroy after lifetime if not picked up
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Bob animation when on the ground
        if (hasLanded)
        {
            bobTimer += Time.deltaTime * bobSpeed;
            float yOffset = Mathf.Sin(bobTimer) * bobAmount;
            transform.position = landedPosition + new Vector3(0, yOffset, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Land on ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            hasLanded = true;
            landedPosition = transform.position;
            
            // Disable physics once landed
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.linearVelocity = Vector2.zero;
            }
        }
        
        // Check for player pickup via collision
        if (collision.gameObject.CompareTag("Player"))
        {
            TryPickup(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check for player pickup via trigger
        if (other.CompareTag("Player"))
        {
            TryPickup(other.gameObject);
        }
    }

    private void TryPickup(GameObject playerObj)
    {
        // Try to find PlayerController on the object or its parent
        PlayerController player = playerObj.GetComponent<PlayerController>();
        if (player == null)
        {
            player = playerObj.GetComponentInParent<PlayerController>();
        }

        if (player != null)
        {
            player.AddAmmo(ammoAmount);
            Debug.Log($"Player picked up supply drop! +{ammoAmount} ammo");
            Destroy(gameObject);
        }
    }
}
