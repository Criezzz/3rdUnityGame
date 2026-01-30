using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI References")]
    public Slider healthSlider;
    public Image fillImage;
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;
    public float lowHealthThreshold = 0.3f;

    [Header("Damage Feedback")]
    public float flashDuration = 0.1f;
    public Color damageColor = Color.red;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Events")]
    public UnityEvent onDeath;
    public UnityEvent<float> onHealthChanged; // Passes health percentage

    [Header("Victory")]
    public GameObject victoryPanel;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        UpdateHealthUI();

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"Boss took {damage} damage! Health: {currentHealth}/{maxHealth}");

        UpdateHealthUI();
        StartCoroutine(DamageFlash());

        // Invoke health changed event
        float healthPercent = currentHealth / maxHealth;
        onHealthChanged?.Invoke(healthPercent);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        float healthPercent = currentHealth / maxHealth;

        if (healthSlider != null)
        {
            healthSlider.value = healthPercent;
        }

        if (fillImage != null)
        {
            // Scale the fill bar based on health
            fillImage.fillAmount = healthPercent;
            
            // Change color based on health
            if (healthPercent <= lowHealthThreshold)
            {
                fillImage.color = lowHealthColor;
            }
            else
            {
                fillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, 
                    (healthPercent - lowHealthThreshold) / (1f - lowHealthThreshold));
            }
        }
    }

    private IEnumerator DamageFlash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Boss Defeated!");

        // Invoke death event
        onDeath?.Invoke();

        // Stop boss behavior
        BossController1 controller = GetComponent<BossController1>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.enabled = false;
        }

        // Show victory
        StartCoroutine(ShowVictory());
    }

    private IEnumerator ShowVictory()
    {
        // Optional: Play death animation or effect
        yield return new WaitForSeconds(1f);

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        // Destroy boss after delay (optional)
        // Destroy(gameObject, 2f);
    }

    // For testing in editor
    [ContextMenu("Test Take 10 Damage")]
    public void TestDamage()
    {
        TakeDamage(10f);
    }
}
