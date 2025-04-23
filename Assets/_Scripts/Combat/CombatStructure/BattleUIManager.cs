using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    [Header("Main UI Components")]
    public GameObject battlePanel;
    public GameObject commandPanel;
    public GameObject targetSelectionPanel;
    
    [Header("Player Information")]
    public GameObject[] playerInfoPanels;
    public TextMeshProUGUI[] playerNames;
    public Slider[] playerHPSliders;
    public TextMeshProUGUI[] playerHPTexts;
    public Slider[] playerMPSliders;
    public TextMeshProUGUI[] playerMPTexts;
    public Image[] playerEchoesGauges;
    
    [Header("Action Buttons")]
    public Button attackButton;
    public Button skillButton;
    public Button itemButton;
    public Button defendButton;
    public Button echoesButton;
    
    [Header("Target Selection")]
    public GameObject targetButtonPrefab;
    public Transform targetButtonContainer;
    
    [Header("Messages")]
    public TextMeshProUGUI battleMessageText;
    public float messageDisplayTime = 2f;
    
    [Header("Turn Order Display")]
    public GameObject turnOrderPanel;
    public GameObject turnIconPrefab;
    public Transform turnIconContainer;
    
    [Header("Addition System")]
    public GameObject additionPanel;
    public Slider additionTimingSlider;
    public Image hitMarkerPrefab;
    public Transform hitMarkersContainer;
    
    // Reference to battle manager
    private BattleManager battleManager;
    
    // Start is called before the first frame update
    void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
        
        // Set up button listeners
        attackButton.onClick.AddListener(() => OnAttackButton());
        skillButton.onClick.AddListener(() => OnSkillButton());
        itemButton.onClick.AddListener(() => OnItemButton());
        defendButton.onClick.AddListener(() => OnDefendButton());
        echoesButton.onClick.AddListener(() => OnEchoesButton());
        
        // Hide panels initially
        commandPanel.SetActive(false);
        targetSelectionPanel.SetActive(false);
        additionPanel.SetActive(false);
    }
    
    // Initialize UI with character data
    public void InitializeUI(List<BattleCharacter> characters)
    {
        int playerIndex = 0;
        
        foreach (BattleCharacter character in characters)
        {
            if (character.isPlayerCharacter && playerIndex < playerInfoPanels.Length)
            {
                // Set up player panel
                playerInfoPanels[playerIndex].SetActive(true);
                playerNames[playerIndex].text = character.characterName;
                
                // Set up HP
                playerHPSliders[playerIndex].maxValue = character.maxHP;
                playerHPSliders[playerIndex].value = character.currentHP;
                playerHPTexts[playerIndex].text = $"{character.currentHP}/{character.maxHP}";
                
                // Set up MP
                playerMPSliders[playerIndex].maxValue = character.maxMP;
                playerMPSliders[playerIndex].value = character.currentMP;
                playerMPTexts[playerIndex].text = $"{character.currentMP}/{character.maxMP}";
                
                // Set up Echoes gauge
                if (playerEchoesGauges[playerIndex] != null)
                {
                    playerEchoesGauges[playerIndex].fillAmount = character.echoesEnergy / character.maxEchoesEnergy;
                }
                
                playerIndex++;
            }
        }
        
        // Disable unused player panels
        for (int i = playerIndex; i < playerInfoPanels.Length; i++)
        {
            playerInfoPanels[i].SetActive(false);
        }
    }
    
    // Update UI with current character data
    public void UpdateUI(List<BattleCharacter> characters)
    {
        int playerIndex = 0;
        
        foreach (BattleCharacter character in characters)
        {
            if (character.isPlayerCharacter && playerIndex < playerInfoPanels.Length)
            {
                // Update HP
                playerHPSliders[playerIndex].value = character.currentHP;
                playerHPTexts[playerIndex].text = $"{character.currentHP}/{character.maxHP}";
                
                // Update MP
                playerMPSliders[playerIndex].value = character.currentMP;
                playerMPTexts[playerIndex].text = $"{character.currentMP}/{character.maxMP}";
                
                // Update Echoes gauge
                if (playerEchoesGauges[playerIndex] != null)
                {
                    playerEchoesGauges[playerIndex].fillAmount = character.echoesEnergy / character.maxEchoesEnergy;
                }
                
                // Show Echoes form if active
                if (character.isEchoesForm)
                {
                    // Visual indication for Echoes form
                    // e.g., playerInfoPanels[playerIndex].GetComponent<Image>().color = Color.cyan;
                }
                
                playerIndex++;
            }
        }
    }
    
    // Show player command buttons
    public void ShowCommandPanel(BattleCharacter character)
    {
        commandPanel.SetActive(true);
        
        // Enable/disable buttons based on available actions
        attackButton.interactable = true;
        skillButton.interactable = character.currentMP > 0;
        itemButton.interactable = true; // Enable based on inventory later
        defendButton.interactable = true;
        echoesButton.interactable = character.CanTransform();
    }
    
    // Hide command panel
    public void HideCommandPanel()
    {
        commandPanel.SetActive(false);
    }
    
    // Show target selection for an action
    public void ShowTargetSelection(List<BattleCharacter> targets, System.Action<BattleCharacter> onTargetSelected)
    {
        // Clear existing target buttons
        foreach (Transform child in targetButtonContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create button for each target
        foreach (BattleCharacter target in targets)
        {
            GameObject buttonObj = Instantiate(targetButtonPrefab, targetButtonContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            
            // Set button text
            buttonText.text = target.characterName;
            
            // Set button click action
            button.onClick.AddListener(() => {
                onTargetSelected(target);
                HideTargetSelection();
            });
        }
        
        targetSelectionPanel.SetActive(true);
        commandPanel.SetActive(false);
    }
    
    // Hide target selection panel
    public void HideTargetSelection()
    {
        targetSelectionPanel.SetActive(false);
    }
    
    // Show battle message
    public IEnumerator ShowBattleMessage(string message)
    {
        battleMessageText.text = message;
        battleMessageText.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(messageDisplayTime);
        
        battleMessageText.gameObject.SetActive(false);
    }
    
    // Update turn order display
    public void UpdateTurnOrderDisplay(List<BattleCharacter> turnOrder)
    {
        // Clear existing icons
        foreach (Transform child in turnIconContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create icon for each character in turn order
        for (int i = 0; i < turnOrder.Count && i < 5; i++) // Show next 5 turns
        {
            GameObject iconObj = Instantiate(turnIconPrefab, turnIconContainer);
            Image icon = iconObj.GetComponent<Image>();
            
            // Set icon image based on character
            if (turnOrder[i].portrait != null)
            {
                icon.sprite = turnOrder[i].portrait;
            }
            
            // Add outline for current actor
            if (i == 0)
            {
                // Highlight current actor
                iconObj.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }
        }
    }
    
    // Set up Addition timing system
    public IEnumerator ShowAdditionSequence(Addition addition, System.Action<int> onComplete)
    {
        // Set up addition panel
        additionPanel.SetActive(true);
        
        // Clear existing hit markers
        foreach (Transform child in hitMarkersContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create hit markers for each timing
        for (int i = 0; i < addition.hitCount; i++)
        {
            Image marker = Instantiate(hitMarkerPrefab, hitMarkersContainer);
            RectTransform rect = marker.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(addition.timingWindows[i], 0);
            rect.anchorMax = new Vector2(addition.timingWindows[i], 1);
            rect.anchoredPosition = Vector2.zero;
        }
        
        int successfulHits = 0;
        
        // Start the timing bar animation
        additionTimingSlider.value = 0;
        
        // For each hit in the addition
        for (int hit = 0; hit < addition.hitCount; hit++)
        {
            bool hitRegistered = false;
            float startTime = Time.time;
            float duration = 1.5f; // Adjust based on difficulty
            
            // Animate timing bar
            while (Time.time - startTime < duration && !hitRegistered)
            {
                float progress = (Time.time - startTime) / duration;
                additionTimingSlider.value = progress;
                
                // Check for player input
                if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space))
                {
                    // Check if input is within timing window
                    float inputTime = progress;
                    bool success = Mathf.Abs(inputTime - addition.timingWindows[hit]) < addition.successMargin;
                    
                    if (success)
                    {
                        successfulHits++;
                        // Visual feedback for success
                        // e.g., flash the marker green
                    }
                    else
                    {
                        // Visual feedback for failure
                        // e.g., flash the marker red
                    }
                    
                    hitRegistered = true;
                }
                
                yield return null;
            }
            
            // If player didn't hit in time
            if (!hitRegistered)
            {
                // Visual feedback for miss
                // e.g., flash the marker red
            }
            
            // Small pause between hits
            yield return new WaitForSeconds(0.3f);
        }
        
        // Hide addition panel
        additionPanel.SetActive(false);
        
        // Call completion callback with number of successful hits
        onComplete(successfulHits);
    }
    
    // Button handlers
    void OnAttackButton()
    {
        battleManager.OnPlayerActionSelected("Attack");
    }
    
    void OnSkillButton()
    {
        battleManager.OnPlayerActionSelected("Skill");
    }
    
    void OnItemButton()
    {
        battleManager.OnPlayerActionSelected("Item");
    }
    
    void OnDefendButton()
    {
        battleManager.OnPlayerActionSelected("Defend");
    }
    
    void OnEchoesButton()
    {
        battleManager.OnPlayerActionSelected("Echoes");
    }
}