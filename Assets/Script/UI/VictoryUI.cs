using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Self-configuring Victory panel - sets up all UI elements automatically
/// </summary>
public class VictoryUI : MonoBehaviour
{
    private Image backgroundImage;
    private TextMeshProUGUI titleText;
    private Button playAgainButton;
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
        if (!isSetup)
        {
            SetupPanel();
            isSetup = true;
        }
    }
    
    private void SetupPanel()
    {
        // Setup this panel as full screen overlay
        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
        }
        
        // Setup background image with semi-transparent dark green color
        backgroundImage = GetComponent<Image>();
        if (backgroundImage != null)
        {
            backgroundImage.color = new Color(0f, 0.2f, 0f, 0.85f); // Dark green
            backgroundImage.raycastTarget = true;
        }
        
        // Find or setup title text
        Transform titleTransform = transform.Find("VictoryText");
        if (titleTransform != null)
        {
            titleText = titleTransform.GetComponent<TextMeshProUGUI>();
            if (titleText != null)
            {
                titleText.text = "VICTORY!";
                titleText.fontSize = 20;
                titleText.color = new Color(1f, 0.9f, 0.2f, 1f); // Gold color
                titleText.alignment = TextAlignmentOptions.Center;
                titleText.fontStyle = FontStyles.Bold;
                
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
        Transform buttonTransform = transform.Find("PlayAgainButton");
        if (buttonTransform != null)
        {
            playAgainButton = buttonTransform.GetComponent<Button>();
            buttonImage = buttonTransform.GetComponent<Image>();
            
            if (buttonImage != null)
            {
                buttonImage.color = new Color(0.2f, 0.8f, 0.2f, 1f); // Green color
            }
            
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
                    buttonText.text = "PLAY AGAIN";
                    buttonText.fontSize = 10;
                    buttonText.color = Color.white;
                    buttonText.alignment = TextAlignmentOptions.Center;
                    buttonText.fontStyle = FontStyles.Bold;
                    
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
            if (playAgainButton != null)
            {
                playAgainButton.onClick.RemoveAllListeners();
                playAgainButton.onClick.AddListener(OnPlayAgainClicked);
            }
        }
    }
    
    public void Show()
    {
        Debug.Log("VictoryUI.Show() called");
        gameObject.SetActive(true);
        
        if (!isSetup)
        {
            SetupPanel();
            isSetup = true;
        }
        
        Time.timeScale = 0f;
    }
    
    public void OnPlayAgainClicked()
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
