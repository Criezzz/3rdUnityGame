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
    public RectTransform healthBarFill;  // Drag BossHP_Fill here

    [Header("Damage Feedback")]
    public float flashDuration = 0.1f;
    public Color damageColor = Color.red;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Events")]
    public UnityEvent onDeath;
    public UnityEvent<float> onHealthChanged;

    [Header("Victory")]
    public GameObject victoryPanel;

    private bool isDead = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;

        // Health bar starts at full
        UpdateHealthUI();

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
        if (healthBarFill != null)
        {
            // Scale the health bar width based on health percentage
            float healthPercent = currentHealth / maxHealth;
            healthBarFill.localScale = new Vector3(healthPercent, 1f, 1f);
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
