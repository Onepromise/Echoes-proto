using System.Collections.Generic;
using UnityEngine;

public class TurnOrderSystem : MonoBehaviour
{
    // All participants in battle
    private List<BattleCharacter> allCharacters = new List<BattleCharacter>();
    
    // Current turn queue
    private Queue<BattleCharacter> turnQueue = new Queue<BattleCharacter>();
    
    public void SetupTurnOrder(BattleCharacter[] playerParty, BattleCharacter[] enemyParty)
    {
        // Clear any existing data
        allCharacters.Clear();
        turnQueue.Clear();
        
        // Add all characters to the list
        allCharacters.AddRange(playerParty);
        allCharacters.AddRange(enemyParty);
        
        // Calculate initial turn order based on speed
        CalculateTurnOrder();
    }
    
    private void CalculateTurnOrder()
    {
        // Sort by speed (higher speed acts first)
        allCharacters.Sort((a, b) => b.speed.CompareTo(a.speed));
        
        // Build turn queue
        foreach (var character in allCharacters)
        {
            if (!character.IsDead())
            {
                turnQueue.Enqueue(character);
            }
        }
    }
    
    public BattleCharacter GetNextCharacter()
    {
        // If queue is empty, recalculate turn order
        if (turnQueue.Count == 0)
        {
            CalculateTurnOrder();
            
            // If still empty (everyone is dead), return null
            if (turnQueue.Count == 0)
                return null;
        }
        
        // Get next character's turn
        BattleCharacter nextCharacter = turnQueue.Dequeue();
        
        return nextCharacter;
    }
    
    // Called when a character's status changes (speed buff/debuff, etc)
    public void RecalculateTurnOrder()
    {
        List<BattleCharacter> remainingCharacters = new List<BattleCharacter>(turnQueue);
        turnQueue.Clear();
        
        // Re-sort remaining characters
        remainingCharacters.Sort((a, b) => b.speed.CompareTo(a.speed));
        
        // Rebuild queue
        foreach (var character in remainingCharacters)
        {
            if (!character.IsDead())
            {
                turnQueue.Enqueue(character);
            }
        }
    }
}