using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnOrderSystem : MonoBehaviour
{
    // Reference to characters in battle
    private List<BattleCharacter> allCharacters = new List<BattleCharacter>();

    // Queue for determining who acts next
    private List<BattleCharacter> turnQueue = new List<BattleCharacter>();

    // Current ATB (Active Time Battle) values
    private Dictionary<BattleCharacter, float> atbValues = new Dictionary<BattleCharacter, float>();

    // ATB configuration
    public float atbMaxValue = 100f;
    public float baseATBFillRate = 10f;

    // Visual references (optional)
    public Transform turnOrderDisplayParent;
    public GameObject turnOrderIconPrefab;
    private Dictionary<BattleCharacter, GameObject> turnOrderIcons = new Dictionary<BattleCharacter, GameObject>();

    private float atbFillSpeedMultiplier = 5f;

    // Initialize system with characters
    // Update this method in your TurnOrderSystem.cs
    public void InitializeTurnOrder(List<BattleCharacter> characters)
    {
        // Clear previous data - keep this as is
        allCharacters.Clear();
        turnQueue.Clear();
        atbValues.Clear();

        // Store all participants
        allCharacters.AddRange(characters);

        // Initialize ATB values with simple, direct calculations
        foreach (BattleCharacter character in allCharacters)
        {
            // Simpler initial value calculation
            float initialValue = character.speed * 0.5f;
            atbValues[character] = Mathf.Clamp(initialValue, 0, atbMaxValue * 0.5f);

            // Only create visual elements if needed - avoid doing this in battle setup
            if (turnOrderDisplayParent != null && turnOrderIconPrefab != null && !turnOrderIcons.ContainsKey(character))
            {
                GameObject icon = Instantiate(turnOrderIconPrefab, turnOrderDisplayParent);
                turnOrderIcons[character] = icon;
            }
        }

        // Update display once at the end, not for each character
        UpdateTurnOrderDisplay();
    }

    // Update ATB values each frame
    public void UpdateATB(float deltaTime)
    {
        // If no characters are ready to act, fill ATB gauges faster
        float speedMultiplier = (turnQueue.Count == 0) ? atbFillSpeedMultiplier : 1f;

        // Process all characters
        foreach (BattleCharacter character in allCharacters)
        {
            // Skip dead characters
            if (character.IsDead()) continue;

            // Calculate how fast this character's ATB fills based on speed
            float fillRate = baseATBFillRate * (character.speed / 100f);

            // Ensure minimum fill rate
            fillRate = Mathf.Max(fillRate, baseATBFillRate * 0.5f);

            // Apply speed multiplier to accelerate gauge filling when needed
            fillRate *= speedMultiplier;

            // Update ATB value
            if (atbValues.ContainsKey(character))
            {
                atbValues[character] += fillRate * deltaTime;

                // Check if character is ready to act
                if (atbValues[character] >= atbMaxValue)
                {
                    atbValues[character] = atbMaxValue;
                    if (!turnQueue.Contains(character))
                    {
                        turnQueue.Add(character);
                    }
                }
            }
        }

        // Update the visual display
        UpdateTurnOrderDisplay();
    }

    // Get the next character who can act
    public BattleCharacter GetNextCharacter()
    {
        if (turnQueue.Count > 0)
        {
            BattleCharacter character = turnQueue[0];
            turnQueue.RemoveAt(0);

            // Reset ATB for this character
            atbValues[character] = 0;

            return character;
        }

        return null;
    }

    // Check if any character is ready to act
    public bool IsAnyCharacterReady()
    {
        return turnQueue.Count > 0;
    }

    // Apply speed changes and update the system
    public void UpdateCharacterSpeed(BattleCharacter character, int newSpeed)
    {
        character.speed = newSpeed;

        // If we update the visual display based on predicted turn order
        UpdateTurnOrderDisplay();
    }

    // Updates visual representation of turn order (optional)
    private void UpdateTurnOrderDisplay()
    {
        if (turnOrderDisplayParent == null) return;

        // Create a list for sorting
        List<KeyValuePair<BattleCharacter, float>> sortedByATB = new List<KeyValuePair<BattleCharacter, float>>();

        foreach (var kvp in atbValues)
        {
            if (!kvp.Key.IsDead()) // Only include living characters
            {
                sortedByATB.Add(kvp);
            }
        }

        // Sort by ATB value (descending)
        sortedByATB.Sort((a, b) => b.Value.CompareTo(a.Value));

        // Position icons based on turn order
        for (int i = 0; i < sortedByATB.Count; i++)
        {
            BattleCharacter character = sortedByATB[i].Key;

            if (turnOrderIcons.ContainsKey(character))
            {
                // Set position in the display
                RectTransform rectTransform = turnOrderIcons[character].GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(i * 80, 0);

                // Optionally, show ATB fill amount
                // turnOrderIcons[character].GetComponent<Slider>().value = sortedByATB[i].Value / atbMaxValue;
            }
        }
    }

    // Method to handle status effects that manipulate turn order
    public void ApplyHasteOrSlow(BattleCharacter character, float multiplier, int turns)
    {
        // Store the effect in the character's status effects
        // Actual implementation depends on your status effect system

        // Immediately update speed - this is simplified, your system might be more complex
        character.speed = Mathf.RoundToInt(character.speed * multiplier);

        // Update display
        UpdateTurnOrderDisplay();
    }

    public void ForceFirstTurn()
    {
        if (turnQueue.Count == 0)
        {
            //sort characters by speed 
            allCharacters.Sort((a, b) => b.speed.CompareTo(a.speed));

            foreach (var character in allCharacters)
            {
                if (!character.IsDead())
                {
                    turnQueue.Add(character);
                    atbValues[character] = atbMaxValue;
                }
            }

        }
    }
}