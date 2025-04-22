using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
   public enum BattleState
   {
        Start,
        PlayerTurn,
        EnemyTurn,
        Victory,
        Defeat
   }

   public BattleState currentState;

    //Reference to the battle positions
   public Transform[] playerPositions;
   public Transform[] enemyPositions;

   private void Start() 
   {
        currentState  = BattleState.Start;
        StartCoroutine(SetupBattle());

   }

   IEnumerator SetupBattle()
   {
        //setup spawning in characters, animations, etc
        yield return new WaitForSeconds(1f);

        currentState = BattleState.PlayerTurn;
        Debug.Log("Player's turn");
   }

   private void Update() 
   {
        if(Input.GetKeyDown(KeyCode.Space) && currentState == BattleState.PlayerTurn)
        {
            currentState = BattleState.EnemyTurn;
            Debug.Log("Enemy's turn");
            StartCoroutine(EnemyTurn());
        }
   }

   IEnumerator EnemyTurn()
   {
    yield return new WaitForSeconds(1f);
    currentState = BattleState.PlayerTurn;
    Debug.Log("Player's turn again");
   }
}
