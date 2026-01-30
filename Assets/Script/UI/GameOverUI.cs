using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// Self-configuring Game Over panel - sets up all UI elements automatically
/// </summary>
public class GameOverUI : MonoBehaviour
{
    private Image backgroundImage;
    private TextMeshProUGUI titleText;
    private Button tryAgainButton;
    private Image buttonImage;
    private TextMeshProUGUI buttonText;
    private bool isSetup = false;
    
    void Awake()
    {
        SetupPanel();
        isSetup = true;
    }
    
    void OnEnable()
    {
        // If we're being enabled and not yet set up, do setup now
        if (!isSetup)
        {
            SetupPanel();
            isSetup = true;
        }
    }
    
    void Start()
    {
        // Note: If this panel starts inactive, Start won't run until it's activated.
        // The setup will happen in OnEnable instead.
    }
    
    private void SetupPanel()
    {
        // Setup this panel as full screen overlay
        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            // Stretch to fill entire parent (which should be the Canvas)
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
            
            Debug.Log($"GameOverPanel RectTransform configured: anchorMin={rect.anchorMin}, anchorMax={rect.anchorMax}, sizeDelta={rect.sizeDelta}");
        }
        
        // Setup background image with semi-transparent dark color
        backgroundImage = GetComponent<Image>();
        if (backgroundImage != null)
        {
            backgroundImage.color = new Color(0f, 0f, 0f, 0.8f);
            backgroundImage.raycastTarget = true;  // Block clicks
        }
        
        // Find or setup title text
        Transform titleTransform = transform.Find("GameOverText");
        if (titleTransform != null)
        {
            titleText = titleTransform.GetComponent<TextMeshProUGUI>();
            if (titleText != null)
            {
                titleText.text = "GAME OVER";
                titleText.fontSize = 20;
                titleText.color = Color.white;
                titleText.alignment = TextAlignmentOptions.Center;
                titleText.fontStyle = FontStyles.Bold;
                
                // Position title in upper half
                RectTransform titleRect = titleTransform.GetComponent<RectTransform>();
                if (titleRect != null)
                {
                    titleRect.anchorMin = new Vector2(0, 0.5f);
                    titleRect.anchorMax = new Vector2(1, 0.8f);
                    titleRect.anchoredPosition = Vector2.zero;
                    titleRect.sizeDelta = Vector2.zero;
                }
            }
        }
        
        // Find or setup button
        Transform buttonTransform = transform.Find("TryAgainButton");
        if (buttonTransform != null)
        {
            tryAgainButton = buttonTransform.GetComponent<Button>();
            buttonImage = buttonTransform.GetComponent<Image>();
            
            if (buttonImage != null)
            {
                buttonImage.color = new Color(0.9f, 0.4f, 0.1f, 1f); // Orange color
            }
            
            // Position button in lower half
            RectTransform buttonRect = buttonTransform.GetComponent<RectTransform>();
            if (buttonRect != null)
            {
                buttonRect.anchorMin = new Vector2(0.3f, 0.25f);
                buttonRect.anchorMax = new Vector2(0.7f, 0.4f);
                buttonRect.anchoredPosition = Vector2.zero;
                buttonRect.sizeDelta = Vector2.zero;
            }
            
            // Setup button text
            Transform buttonTextTransform = buttonTransform.Find("ButtonText");
            if (buttonTextTransform != null)
            {
                buttonText = buttonTextTransform.GetComponent<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = "TRY AGAIN";
                    buttonText.fontSize = 10;
                    buttonText.color = Color.white;
                    buttonText.alignment = TextAlignmentOptions.Center;
                    buttonText.fontStyle = FontStyles.Bold;
                    
                    // Stretch button text to fill button
                    RectTransform textRect = buttonTextTransform.GetComponent<RectTransform>();
                    if (textRect != null)
                    {
                        textRect.anchorMin = Vector2.zero;
                        textRect.anchorMax = Vector2.one;
                        textRect.anchoredPosition = Vector2.zero;
                        textRect.sizeDelta = Vector2.zero;
                    }
                }
            }
            
            // Add click listener
            if (tryAgainButton != null)
            {
                tryAgainButton.onClick.RemoveAllListeners();
                tryAgainButton.onClick.AddListener(OnTryAgainClicked);
            }
        }
    }
    
    public void Show()
    {
        Debug.Log("GameOverUI.Show() called");
        gameObject.SetActive(true);
        
        // Setup if not already done (in case Awake didn't run)
        if (!isSetup)
        {
            SetupPanel();
            isSetup = true;
        }
        
        Time.timeScale = 0f;
    }
    
    public void OnTryAgainClicked()
    {
        Time.timeScale = 1f;
        
        // Destroy any DontDestroyOnLoad singletons to reset game state
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
