using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper script to set up UI elements at runtime.
/// Attach to GameUI Canvas, then click "Setup All UI" in context menu (right-click the component).
/// </summary>
[ExecuteInEditMode]
public class UISetupHelper : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public BossHealth bossHealth;
    
    [Header("Created Panels")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public Slider bossHealthSlider;

    void OnEnable()
    {
        // Auto-find references in editor
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (bossHealth == null)
            bossHealth = FindFirstObjectByType<BossHealth>();
    }

    [ContextMenu("Setup Game Over Panel")]
    public void SetupGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            Debug.Log("GameOverPanel already exists!");
            return;
        }

        // Create GameOverPanel
        gameOverPanel = new GameObject("GameOverPanel");
        gameOverPanel.transform.SetParent(transform, false);
        
        // Add RectTransform and stretch to fill
        RectTransform rt = gameOverPanel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;

        // Add dark overlay Image
        Image overlay = gameOverPanel.AddComponent<Image>();
        overlay.color = new Color(0, 0, 0, 0.7f); // Semi-transparent black

        // Create "GAME OVER" text
        GameObject textObj = new GameObject("GameOverText");
        textObj.transform.SetParent(gameOverPanel.transform, false);
        RectTransform textRt = textObj.AddComponent<RectTransform>();
        textRt.anchorMin = new Vector2(0.5f, 0.6f);
        textRt.anchorMax = new Vector2(0.5f, 0.6f);
        textRt.sizeDelta = new Vector2(400, 100);
        textRt.anchoredPosition = Vector2.zero;

        // Try to add TextMeshProUGUI, fallback to Text
        var tmpText = textObj.AddComponent<TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = "GAME OVER";
            tmpText.fontSize = 72;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.color = Color.white;
        }

        // Create "Try Again" button
        GameObject buttonObj = new GameObject("TryAgainButton");
        buttonObj.transform.SetParent(gameOverPanel.transform, false);
        RectTransform btnRt = buttonObj.AddComponent<RectTransform>();
        btnRt.anchorMin = new Vector2(0.5f, 0.35f);
        btnRt.anchorMax = new Vector2(0.5f, 0.35f);
        btnRt.sizeDelta = new Vector2(200, 60);
        btnRt.anchoredPosition = Vector2.zero;

        Image btnImage = buttonObj.AddComponent<Image>();
        btnImage.color = new Color(0.2f, 0.6f, 0.2f, 1f); // Green button

        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = btnImage;
        
        // Add button text
        GameObject btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(buttonObj.transform, false);
        RectTransform btnTextRt = btnTextObj.AddComponent<RectTransform>();
        btnTextRt.anchorMin = Vector2.zero;
        btnTextRt.anchorMax = Vector2.one;
        btnTextRt.sizeDelta = Vector2.zero;
        btnTextRt.anchoredPosition = Vector2.zero;

        var btnTmpText = btnTextObj.AddComponent<TextMeshProUGUI>();
        if (btnTmpText != null)
        {
            btnTmpText.text = "Try Again";
            btnTmpText.fontSize = 32;
            btnTmpText.alignment = TextAlignmentOptions.Center;
            btnTmpText.color = Color.white;
        }

        // Connect button to PlayerHealth.RestartGame
        if (playerHealth != null)
        {
            button.onClick.AddListener(playerHealth.RestartGame);
        }

        // Start disabled
        gameOverPanel.SetActive(false);

        // Assign to PlayerHealth
        if (playerHealth != null)
        {
            playerHealth.gameOverPanel = gameOverPanel;
        }

        Debug.Log("GameOverPanel created successfully!");
    }

    [ContextMenu("Setup Victory Panel")]
    public void SetupVictoryPanel()
    {
        if (victoryPanel != null)
        {
            Debug.Log("VictoryPanel already exists!");
            return;
        }

        // Create VictoryPanel
        victoryPanel = new GameObject("VictoryPanel");
        victoryPanel.transform.SetParent(transform, false);
        
        RectTransform rt = victoryPanel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;

        Image overlay = victoryPanel.AddComponent<Image>();
        overlay.color = new Color(0, 0.2f, 0, 0.7f); // Semi-transparent dark green

        // Create "VICTORY" text
        GameObject textObj = new GameObject("VictoryText");
        textObj.transform.SetParent(victoryPanel.transform, false);
        RectTransform textRt = textObj.AddComponent<RectTransform>();
        textRt.anchorMin = new Vector2(0.5f, 0.6f);
        textRt.anchorMax = new Vector2(0.5f, 0.6f);
        textRt.sizeDelta = new Vector2(400, 100);
        textRt.anchoredPosition = Vector2.zero;

        var tmpText = textObj.AddComponent<TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = "VICTORY!";
            tmpText.fontSize = 72;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.color = Color.yellow;
        }

        // Create "Play Again" button
        GameObject buttonObj = new GameObject("PlayAgainButton");
        buttonObj.transform.SetParent(victoryPanel.transform, false);
        RectTransform btnRt = buttonObj.AddComponent<RectTransform>();
        btnRt.anchorMin = new Vector2(0.5f, 0.35f);
        btnRt.anchorMax = new Vector2(0.5f, 0.35f);
        btnRt.sizeDelta = new Vector2(200, 60);
        btnRt.anchoredPosition = Vector2.zero;

        Image btnImage = buttonObj.AddComponent<Image>();
        btnImage.color = new Color(0.2f, 0.6f, 0.2f, 1f);

        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = btnImage;
        
        GameObject btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(buttonObj.transform, false);
        RectTransform btnTextRt = btnTextObj.AddComponent<RectTransform>();
        btnTextRt.anchorMin = Vector2.zero;
        btnTextRt.anchorMax = Vector2.one;
        btnTextRt.sizeDelta = Vector2.zero;
        btnTextRt.anchoredPosition = Vector2.zero;

        var btnTmpText = btnTextObj.AddComponent<TextMeshProUGUI>();
        if (btnTmpText != null)
        {
            btnTmpText.text = "Play Again";
            btnTmpText.fontSize = 32;
            btnTmpText.alignment = TextAlignmentOptions.Center;
            btnTmpText.color = Color.white;
        }

        if (playerHealth != null)
        {
            button.onClick.AddListener(playerHealth.RestartGame);
        }

        victoryPanel.SetActive(false);

        if (bossHealth != null)
        {
            bossHealth.victoryPanel = victoryPanel;
        }

        Debug.Log("VictoryPanel created successfully!");
    }

    [ContextMenu("Setup Boss Health Slider")]
    public void SetupBossHealthSlider()
    {
        // Find or create BossHealthUI
        Transform bossHealthUI = transform.Find("BossHealthUI");
        if (bossHealthUI == null)
        {
            GameObject bossHealthUIObj = new GameObject("BossHealthUI");
            bossHealthUIObj.transform.SetParent(transform, false);
            bossHealthUI = bossHealthUIObj.transform;
            
            RectTransform rt = bossHealthUIObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0, -20);
            rt.sizeDelta = new Vector2(400, 30);
        }

        // Clear existing children
        foreach (Transform child in bossHealthUI)
        {
            DestroyImmediate(child.gameObject);
        }

        // Create Slider structure
        GameObject sliderObj = new GameObject("BossSlider");
        sliderObj.transform.SetParent(bossHealthUI, false);
        RectTransform sliderRt = sliderObj.AddComponent<RectTransform>();
        sliderRt.anchorMin = Vector2.zero;
        sliderRt.anchorMax = Vector2.one;
        sliderRt.sizeDelta = Vector2.zero;
        sliderRt.anchoredPosition = Vector2.zero;

        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 1;
        slider.interactable = false; // Not user-controllable

        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(sliderObj.transform, false);
        RectTransform bgRt = bgObj.AddComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.sizeDelta = Vector2.zero;
        bgRt.anchoredPosition = Vector2.zero;
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.15f, 0.15f, 0.15f, 1f);

        // Fill Area
        GameObject fillAreaObj = new GameObject("Fill Area");
        fillAreaObj.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRt = fillAreaObj.AddComponent<RectTransform>();
        fillAreaRt.anchorMin = new Vector2(0, 0.15f);
        fillAreaRt.anchorMax = new Vector2(1, 0.85f);
        fillAreaRt.sizeDelta = new Vector2(-10, 0);
        fillAreaRt.anchoredPosition = new Vector2(-5, 0);

        // Fill
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(fillAreaObj.transform, false);
        RectTransform fillRt = fillObj.AddComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.sizeDelta = Vector2.zero;
        fillRt.anchoredPosition = Vector2.zero;
        Image fillImage = fillObj.AddComponent<Image>();
        fillImage.color = new Color(0.8f, 0.1f, 0.1f, 1f); // Red

        // Configure slider
        slider.fillRect = fillRt;
        slider.targetGraphic = fillImage;

        bossHealthSlider = slider;

        // Assign to BossHealth
        if (bossHealth != null)
        {
            bossHealth.healthSlider = slider;
            bossHealth.fillImage = fillImage;
        }

        Debug.Log("Boss Health Slider created successfully!");
    }

    [ContextMenu("Setup All UI")]
    public void SetupAllUI()
    {
        SetupGameOverPanel();
        SetupVictoryPanel();
        SetupBossHealthSlider();
        Debug.Log("All UI setup complete!");
    }
}
