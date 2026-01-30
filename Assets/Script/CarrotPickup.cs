using UnityEngine;

public class CarrotPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public int ammoAmount = 1;
    public float lifetime = 10f;
    public float bobSpeed = 2f;
    public float bobAmount = 0.2f;

    private Vector3 startPosition;
    private float timer;

    void Start()
    {
        startPosition = transform.position;
        
        // Destroy after lifetime
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Bob up and down animation
        timer += Time.deltaTime * bobSpeed;
        float yOffset = Mathf.Sin(timer) * bobAmount;
        transform.position = startPosition + new Vector3(0, yOffset, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Try to add ammo to player
            PlayerController player = other.GetComponent<PlayerController>();
            if (player == null)
            {
                player = other.GetComponentInParent<PlayerController>();
            }

            if (player != null)
            {
                player.AddAmmo(ammoAmount);
                Debug.Log($"Player picked up {ammoAmount} carrot ammo!");
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Also handle collision (in case trigger doesn't work)
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player == null)
            {
                player = collision.gameObject.GetComponentInParent<PlayerController>();
            }

            if (player != null)
            {
                player.AddAmmo(ammoAmount);
                Destroy(gameObject);
            }
        }
    }
}
