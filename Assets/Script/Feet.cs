using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feet : MonoBehaviour
{
    public PlayerController player;
    public float stompBounceForce = 10f;
    
    void Awake()
    {
        player = transform.parent.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Boss touch = instant death
        if (IsBoss(collision.gameObject))
        {
            KillPlayer();
            return;
        }
        
        // Check for enemy stomp
        if (IsEnemy(collision.gameObject))
        {
            // Stomp the enemy!
            StompEnemy(collision.gameObject);
            return;
        }
        
        if(collision.gameObject.CompareTag("Ground"))
        {
            Vector3 normal = collision.GetContact(0).normal;

            if (normal == Vector3.up)
            {
                player.onGround = true;
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Boss touch = instant death
        if (IsBoss(collider.gameObject))
        {
            KillPlayer();
            return;
        }
        
        // Also handle trigger collisions for enemies
        if (IsEnemy(collider.gameObject))
        {
            StompEnemy(collider.gameObject);
        }
    }
    
    private bool IsBoss(GameObject obj)
    {
        if (obj.CompareTag("Boss"))
            return true;
        if (obj.transform.parent != null && obj.transform.parent.CompareTag("Boss"))
            return true;
        return false;
    }
    
    private bool IsEnemy(GameObject obj)
    {
        // Exclude boss from stomp
        if (IsBoss(obj))
            return false;
        
        if (obj.CompareTag("Enemy"))
            return true;
        if (obj.transform.parent != null && obj.transform.parent.CompareTag("Enemy"))
            return true;
        // Check layer 8 (Enemy layer) but not Boss
        if (obj.layer == 8)
            return true;
        return false;
    }
    
    private void KillPlayer()
    {
        PlayerHealth health = player?.GetComponent<PlayerHealth>();
        if (health != null && !health.IsDead)
        {
            health.OnDamaged(health.currentHealth);
        }
    }
    
    private void StompEnemy(GameObject enemy)
    {
        // Find root enemy
        GameObject enemyRoot = enemy;
        if (enemy.transform.parent != null && 
            (enemy.transform.parent.CompareTag("Enemy") || enemy.transform.parent.gameObject.layer == 8))
        {
            enemyRoot = enemy.transform.parent.gameObject;
        }
        
        // Bounce player up
        if (player != null && player.rg != null)
        {
            player.rg.linearVelocity = new Vector2(player.rg.linearVelocity.x, 0);
            player.rg.AddForce(Vector2.up * stompBounceForce, ForceMode2D.Impulse);
        }
        
        // Destroy enemy
        Destroy(enemyRoot);
        Debug.Log($"Feet stomped enemy: {enemyRoot.name}");
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            player.onGround = false;
        }
    }
}
