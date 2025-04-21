using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    Start,
    PlayerTurn,
    PlayerAction,
    EnemyTurn,
    Won,
    Lost
}

public class BattleSystem : MonoBehaviour
{
    public BattleState state;
    
    // References to other components
    public GameObject playerPartyPrefab;
    public GameObject enemyPartyPrefab;
    public Transform playerBattleStation;
    public Transform enemyBattleStation;
    
    // Reference to UI and systems
    private BattleUI battleUI;
    private TurnOrderSystem turnOrderSystem;
    private EchoesSystem echoesSystem; // Changed from DragoonSystem
    
    // Battle data
    private BattleCharacter[] playerParty;
    private BattleCharacter[] enemyParty;
    private BattleCharacter currentCharacter;
    
    void Start()
    {
        state = BattleState.Start;
        
        // Get references
        battleUI = FindObjectOfType<BattleUI>();
        turnOrderSystem = GetComponent<TurnOrderSystem>();
        echoesSystem = GetComponent<EchoesSystem>(); // Changed from DragoonSystem
        
        SetupBattle();
    }
    
    void SetupBattle()
    {
        // Instantiate player and enemy parties
        GameObject playerGO = Instantiate(playerPartyPrefab, playerBattleStation);
        GameObject enemyGO = Instantiate(enemyPartyPrefab, enemyBattleStation);
        
        // Get character data
        // (Implementation would depend on how you're storing character data)
        
        // Initialize turn order
        turnOrderSystem.SetupTurnOrder(playerParty, enemyParty);
        
        // Initialize echoes system
        echoesSystem.InitializeEchoesPoints(playerParty);
        
        // Start the battle
        StartCoroutine(BattleSequence());
    }
    
    IEnumerator BattleSequence()
    {
        // Wait for setup animations/effects
        yield return new WaitForSeconds(1f);
        
        // Begin with first character's turn based on speed
        NextTurn();
    }
    
    void NextTurn()
    {
        // Get next character
        currentCharacter = turnOrderSystem.GetNextCharacter();
        
        // Check for battle end
        if (AllPlayersDead())
        {
            state = BattleState.Lost;
            EndBattle(false);
            return;
        }
        else if (AllEnemiesDead())
        {
            state = BattleState.Won;
            EndBattle(true);
            return;
        }
        
        // Update echoes form duration if applicable
        if (IsPlayerCharacter(currentCharacter))
        {
            echoesSystem.UpdateEchoesForm(currentCharacter);
        }
        
        // Handle turn based on character type
        if (IsPlayerCharacter(currentCharacter))
        {
            state = BattleState.PlayerTurn;
            battleUI.ShowActionButtons(currentCharacter);
        }
        else
        {
            state = BattleState.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }
    }
    
    // Helper methods
    bool IsPlayerCharacter(BattleCharacter character)
    {
        return System.Array.IndexOf(playerParty, character) != -1;
    }
    
    bool AllPlayersDead()
    {
        foreach (var character in playerParty)
        {
            if (!character.IsDead())
                return false;
        }
        return true;
    }
    
    bool AllEnemiesDead()
    {
        foreach (var enemy in enemyParty)
        {
            if (!enemy.IsDead())
                return false;
        }
        return true;
    }
    
    void EndBattle(bool playerWon)
    {
        // Handle battle end logic
        // (Implementation here)
    }
    
    // Enemy AI turn
    IEnumerator EnemyTurn()
    {
        // Simple enemy AI would go here
        yield return new WaitForSeconds(1f);
        
        // Move to next turn
        NextTurn();
    }
    
    // Called from UI when player selects actions
    public void OnPlayerAttack()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(PerformPlayerAttack());
    }
    
    public void OnPlayerEchoes() // Changed from OnPlayerDragoon
    {
        if (echoesSystem.CanTransform(currentCharacter))
        {
            state = BattleState.PlayerAction;
            StartCoroutine(PerformEchoesTransformation());
        }
        else
        {
            battleUI.ShowMessage("Not enough Echoes energy!");
        }
    }
    
    // Action coroutines
    IEnumerator PerformPlayerAttack()
    {
        // Get target selection from UI
        // (Implementation here)
        
        // Get current addition
        Addition currentAddition = currentCharacter.availableAdditions[currentCharacter.currentAdditionIndex];
        
        // Show addition sequence
        yield return StartCoroutine(battleUI.ShowAdditionSequence(currentAddition));
        
        // Apply damage based on addition success
        // (Implementation here)
        
        // Move to next turn
        yield return new WaitForSeconds(1f);
        NextTurn();
    }
    
    IEnumerator PerformEchoesTransformation() // Changed from PerformDragoonTransformation
    {
        // Play transformation animation
        // (Implementation here)
        
        // Transform character
        echoesSystem.TransformToEchoesForm(currentCharacter);
        
        // Update UI
        battleUI.UpdateUI(playerParty);
        
        // Move to next turn
        yield return new WaitForSeconds(1f);
        NextTurn();
    }
}