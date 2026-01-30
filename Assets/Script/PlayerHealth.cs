using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("UI References")]
    public Image[] heartImages;  // Array of heart UI images
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public float gameOverDelay = 1f;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    // Called by Body.cs when player takes damage
    public void OnDamaged(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        UpdateHeartsUI();
        
        Debug.Log($"Player took {damage} damage! Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHeartsUI()
    {
        if (heartImages == null || heartImages.Length == 0) return;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null) continue;

            if (i < currentHealth)
            {
                heartImages[i].sprite = fullHeart;
                heartImages[i].enabled = true;
            }
            else
            {
                if (emptyHeart != null)
                {
                    heartImages[i].sprite = emptyHeart;
                    heartImages[i].enabled = true;
                }
                else
                {
                    heartImages[i].enabled = false;
                }
            }
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player Died!");

        // Disable player controls
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        // Show game over after delay
        StartCoroutine(ShowGameOver());
    }

    private IEnumerator ShowGameOver()
    {
        yield return new WaitForSeconds(gameOverDelay);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f; // Pause game
        }
    }

    // Call this from UI button
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }

    // For testing/power-ups
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateHeartsUI();
    }
}
