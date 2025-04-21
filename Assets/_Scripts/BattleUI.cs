using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleUI : MonoBehaviour
{
    // Action selection
    public GameObject actionPanel;
    public Button attackButton;
    public Button itemButton;
    public Button defendButton;
    public Button specialButton;
    public Button echoesButton; // Changed from dragoonButton
    
    // Character info
    public GameObject[] characterInfoPanels;
    public Text[] characterNames;
    public Slider[] hpBars;
    public Slider[] mpBars;
    
    // Addition UI
    public GameObject additionPanel;
    public Slider additionTimingBar;
    
    // Battle messages
    public Text battleMessageText;
    
    // Reference to battle system
    private BattleSystem battleSystem;
    
    void Start()
    {
        battleSystem = FindObjectOfType<BattleSystem>();
        
        // Set up button listeners
        attackButton.onClick.AddListener(() => OnAttackButton());
        itemButton.onClick.AddListener(() => OnItemButton());
        defendButton.onClick.AddListener(() => OnDefendButton());
        specialButton.onClick.AddListener(() => OnSpecialButton());
        echoesButton.onClick.AddListener(() => OnEchoesButton()); // Changed from OnDragoonButton
        
        // Hide addition panel by default
        additionPanel.SetActive(false);
    }
    
    public void UpdateUI(BattleCharacter[] playerParty)
    {
        // Update character info panels
        for (int i = 0; i < playerParty.Length; i++)
        {
            if (i < characterInfoPanels.Length)
            {
                characterNames[i].text = playerParty[i].name;
                hpBars[i].maxValue = playerParty[i].maxHP;
                hpBars[i].value = playerParty[i].currentHP;
                mpBars[i].maxValue = playerParty[i].maxMP;
                mpBars[i].value = playerParty[i].currentMP;
                
                // Show echoes indicator if transformed (changed terminology)
                // (Implementation here)
            }
        }
    }
    
    // Button handlers (to be implemented)
    void OnAttackButton() { /* ... */ }
    void OnItemButton() { /* ... */ }
    void OnDefendButton() { /* ... */ }
    void OnSpecialButton() { /* ... */ }
    void OnEchoesButton() { /* ... */ } // Changed from OnDragoonButton
    
    // For the timed addition system
    public IEnumerator ShowAdditionSequence(Addition addition)
    {
        // Show addition panel
        additionPanel.SetActive(true);
        
        // For each hit in the addition
        for (int hit = 0; hit < addition.hitCount; hit++)
        {
            // Reset timing bar
            additionTimingBar.value = 0;
            
            // Animate timing bar
            float startTime = Time.time;
            float duration = 1.0f; // Adjust based on difficulty
            
            // Wait for the correct timing window
            while (Time.time - startTime < duration)
            {
                float progress = (Time.time - startTime) / duration;
                additionTimingBar.value = progress;
                
                // Check for player input
                if (Input.GetButtonDown("Fire1"))
                {
                    // Check if input is within timing window
                    float inputTime = progress;
                    bool success = Mathf.Abs(inputTime - addition.timingWindows[hit]) < 0.1f;
                    
                    // Visual feedback for hit success/failure
                    // (Implementation here)
                    
                    break;
                }
                
                yield return null;
            }
            
            // Small pause between hits
            yield return new WaitForSeconds(0.3f);
        }
        
        // Hide addition panel when done
        additionPanel.SetActive(false);
    }
}