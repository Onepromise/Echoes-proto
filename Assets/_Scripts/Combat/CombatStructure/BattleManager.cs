using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
     // Battle state enum (unchanged)
     public enum BattleState
     {
          Start,
          PlayerTurn,
          EnemyTurn,
          Victory,
          Defeat
     }

     public BattleState currentState;

     // Reference to the battle positions
     public Transform[] playerPositions;
     public Transform[] enemyPositions;

     // Character data references
     public CharacterData[] playerCharacters;
     public CharacterData[] enemyCharacters;

     // Prefabs for visuals
     public GameObject characterPrefab;

     // Turn system
     public TurnOrderSystem turnOrderSystem;
     private BattleCharacter currentActor;
     private List<BattleCharacter> allBattleCharacters = new List<BattleCharacter>();

     // Visual representations in scene
     private Dictionary<BattleCharacter, GameObject> characterObjects = new Dictionary<BattleCharacter, GameObject>();

     // Start is called before the first frame update
     void Start()
     {
          currentState = BattleState.Start;

          // Get turn order system component
          if (turnOrderSystem == null)
          {
               turnOrderSystem = GetComponent<TurnOrderSystem>();
          }

          StartCoroutine(SetupBattle());
     }

     IEnumerator SetupBattle()
     {
          float startTime = Time.realtimeSinceStartup;
          Debug.Log("Battle starting!");

          CreateAllCharacters();
          Debug.Log($"Character creation took: {Time.realtimeSinceStartup - startTime} seconds");

          startTime = Time.realtimeSinceStartup;
          Debug.Log("Initializing turn order...");

          turnOrderSystem.InitializeTurnOrder(allBattleCharacters);
          Debug.Log($"Turn order initialization took: {Time.realtimeSinceStartup - startTime} seconds");

          //Force first turn
          turnOrderSystem.ForceFirstTurn();

          currentState = BattleState.PlayerTurn;
          ProcessTurns();

          //Debug Monitor how long until first turn appears
          startTime = Time.realtimeSinceStartup;
          yield return null; 
          Debug.Log($"Time between battle setup and first frame: {Time.realtimeSinceStartup - startTime}");

     }

     
     private void CreateAllCharacters()
     {
          // Clear any existing characters
          allBattleCharacters.Clear();
          foreach (var obj in characterObjects.Values)
          {
               Destroy(obj);
          }
          characterObjects.Clear();

          // Create player characters
          for (int i = 0; i < playerCharacters.Length && i < playerPositions.Length; i++)
          {
               BattleCharacter playerChar = playerCharacters[i].CreateBattleCharacter();
               playerChar.battlePosition = playerPositions[i];
               allBattleCharacters.Add(playerChar);

               GameObject charObj = CreateCharacterVisual(playerChar, playerPositions[i]);
               characterObjects[playerChar] = charObj;
          }

          // Create enemy characters
          for (int i = 0; i < enemyCharacters.Length && i < enemyPositions.Length; i++)
          {
               BattleCharacter enemyChar = enemyCharacters[i].CreateBattleCharacter();
               enemyChar.battlePosition = enemyPositions[i];
               allBattleCharacters.Add(enemyChar);

               GameObject charObj = CreateCharacterVisual(enemyChar, enemyPositions[i]);
               characterObjects[enemyChar] = charObj;
          }
     }

     // Create a visual representation of the character in the scene
     private GameObject CreateCharacterVisual(BattleCharacter character, Transform position)
     {
          GameObject charObj;

          if (character.characterPrefab != null)
          {
               // Use the character's specific prefab if available
               charObj = Instantiate(character.characterPrefab, position.position, position.rotation);
          }
          else
          {
               // Otherwise use the default placeholder
               charObj = Instantiate(characterPrefab, position.position, position.rotation);
          }

          // Name the object for easy identification in hierarchy
          charObj.name = character.characterName;

          // Add a TextMesh for displaying character name (helpful for testing)
          GameObject nameObj = new GameObject("NameDisplay");
          nameObj.transform.SetParent(charObj.transform);
          nameObj.transform.localPosition = new Vector3(0, 2, 0);
          TextMesh textMesh = nameObj.AddComponent<TextMesh>();
          textMesh.text = character.characterName;
          textMesh.fontSize = 10;
          textMesh.alignment = TextAlignment.Center;
          textMesh.anchor = TextAnchor.MiddleCenter;

          return charObj;
     }

     private void Update()
     {
          // Only update ATB when we're in appropriate states
          if (currentState == BattleState.PlayerTurn || currentState == BattleState.EnemyTurn)
          {
               turnOrderSystem.UpdateATB(Time.deltaTime);

               // If a character is ready and we're not already processing someone's turn
               if (turnOrderSystem.IsAnyCharacterReady() && currentActor == null)
               {
                    ProcessTurns();
               }
          }

          // DEBUG: Press Space to skip the current turn (for testing)
          if (Input.GetKeyDown(KeyCode.Space) && currentActor != null)
          {
               Debug.Log($"Skipping {currentActor.characterName}'s turn");
               currentActor = null;
          }
     }

     void ProcessTurns()
     {

          float startTime = Time.realtimeSinceStartup;

          // Get the next character who can act
          currentActor = turnOrderSystem.GetNextCharacter();

          if (currentActor != null)
          {
               Debug.Log($"Getting next character tookL {Time.realtimeSinceStartup - startTime} seconds");
               Debug.Log($"Next turn: {currentActor.characterName} (Speed: {currentActor.speed})");

               // Highlight the current actor visually
               if (characterObjects.ContainsKey(currentActor))
               {
                    // TODO: Add visual highlight effect
               }

               // Determine if this is a player or enemy
               if (IsPlayerCharacter(currentActor))
               {
                    currentState = BattleState.PlayerTurn;
                    // Show UI for player to select action
                    ShowPlayerActions(currentActor);
               }
               else
               {
                    currentState = BattleState.EnemyTurn;
                    StartCoroutine(ProcessEnemyTurn(currentActor));
               }
          }
     }

     bool IsPlayerCharacter(BattleCharacter character)
     {
          return character.isPlayerCharacter;
     }

     // Show UI for player actions
     void ShowPlayerActions(BattleCharacter character)
     {
          // Enable and display the action buttons for this character
          Debug.Log($"{character.characterName}'s turn. Select an action.");
     }

     // Process enemy turn
     IEnumerator ProcessEnemyTurn(BattleCharacter enemy)
     {
          Debug.Log($"{enemy.characterName} is taking their turn.");

          // Simple AI logic would go here
          yield return new WaitForSeconds(1.5f);

          // Perform a mock attack on a random player
          BattleCharacter target = GetRandomPlayerCharacter();
          if (target != null)
          {
               Debug.Log($"{enemy.characterName} attacks {target.characterName}!");

               // Simulate damage
               int damage = enemy.CalculatePhysicalDamage(10, target);
               target.TakeDamage(damage);

               Debug.Log($"{target.characterName} takes {damage} damage! HP: {target.currentHP}/{target.maxHP}");

               // Check for defeat
               if (target.IsDead())
               {
                    Debug.Log($"{target.characterName} has been defeated!");
                    // TODO: Handle character defeat visually
               }

               yield return new WaitForSeconds(1.0f);
          }

          // Turn completed, reset current actor
          currentActor = null;

          // Check victory/defeat conditions
          CheckBattleEndConditions();
     }

     // Get a random player character that's still alive
     private BattleCharacter GetRandomPlayerCharacter()
     {
          List<BattleCharacter> alivePlayers = new List<BattleCharacter>();

          foreach (BattleCharacter character in allBattleCharacters)
          {
               if (character.isPlayerCharacter && !character.IsDead())
               {
                    alivePlayers.Add(character);
               }
          }

          if (alivePlayers.Count > 0)
          {
               return alivePlayers[Random.Range(0, alivePlayers.Count)];
          }

          return null;
     }

     // Check if battle should end
     private void CheckBattleEndConditions()
     {
          bool allPlayersDefeated = true;
          bool allEnemiesDefeated = true;

          foreach (BattleCharacter character in allBattleCharacters)
          {
               if (character.isPlayerCharacter && !character.IsDead())
               {
                    allPlayersDefeated = false;
               }

               if (!character.isPlayerCharacter && !character.IsDead())
               {
                    allEnemiesDefeated = false;
               }
          }

          if (allPlayersDefeated)
          {
               currentState = BattleState.Defeat;
               Debug.Log("Battle Lost! All players have been defeated.");
          }
          else if (allEnemiesDefeated)
          {
               currentState = BattleState.Victory;
               Debug.Log("Battle Won! All enemies have been defeated.");
          }
     }

     // Add this method to be called when player selects an action
     public void OnPlayerActionSelected(string actionType)
     {
          if (currentActor == null || !currentActor.isPlayerCharacter) return;

          // Process the selected action
          Debug.Log($"Player selected: {actionType}");

          StartCoroutine(ProcessPlayerAction(actionType));
     }

     // Process player action
     private IEnumerator ProcessPlayerAction(string actionType)
     {
          if (actionType == "Attack")
          {
               // Get a random enemy to attack
               BattleCharacter target = GetRandomEnemyCharacter();

               if (target != null)
               {
                    Debug.Log($"{currentActor.characterName} attacks {target.characterName}!");

                    // Simulate damage
                    int damage = currentActor.CalculatePhysicalDamage(10, target);
                    target.TakeDamage(damage);

                    Debug.Log($"{target.characterName} takes {damage} damage! HP: {target.currentHP}/{target.maxHP}");

                    // Check for defeat
                    if (target.IsDead())
                    {
                         Debug.Log($"{target.characterName} has been defeated!");
                         // TODO: Handle character defeat visually
                    }

                    yield return new WaitForSeconds(1.0f);
               }
          }

          // Turn completed, reset current actor
          currentActor = null;

          // Check victory/defeat conditions
          CheckBattleEndConditions();
     }

     // Get a random enemy character that's still alive
     private BattleCharacter GetRandomEnemyCharacter()
     {
          List<BattleCharacter> aliveEnemies = new List<BattleCharacter>();

          foreach (BattleCharacter character in allBattleCharacters)
          {
               if (!character.isPlayerCharacter && !character.IsDead())
               {
                    aliveEnemies.Add(character);
               }
          }

          if (aliveEnemies.Count > 0)
          {
               return aliveEnemies[Random.Range(0, aliveEnemies.Count)];
          }

          return null;
     }
}