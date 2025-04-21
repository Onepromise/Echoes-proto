using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleTransitionManager : MonoBehaviour
{
    // Singleton pattern
    public static BattleTransitionManager Instance;
    
    // Data to pass between scenes
    private EnemyGroup currentEnemyGroup;
    
    void Awake()
    {
        // Ensure singleton persists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Called from world scene to initiate battle
    public void StartBattle(EnemyGroup enemyGroup)
    {
        // Store encounter data
        currentEnemyGroup = enemyGroup;
        
        // Play transition effect (implementation will vary)
        StartCoroutine(TransitionToBattle());
    }
    
    // Get the random encounter data in the battle scene
    public EnemyGroup GetCurrentBattle()
    {
        return currentEnemyGroup;
    }
    
    private IEnumerator TransitionToBattle()
    {
        // Play transition animation
        // (Implementation here)
        
        yield return new WaitForSeconds(1.0f);
        
        // Load battle scene
        SceneManager.LoadScene("BattleScene");
    }
    
    // Called when battle ends
    public void EndBattle(bool playerWon)
    {
        if (playerWon)
        {
            // Handle victory rewards
            // (Implementation here)
        }
        
        // Return to previous scene
        SceneManager.LoadScene("WorldScene");
    }
}

// Define enemy encounters
[System.Serializable]
public class EnemyGroup
{
    public string encounterName;
    public List<EnemyData> enemies = new List<EnemyData>();
    public bool isBossFight = false;
    public bool allowEscape = true;
}

[System.Serializable]
public class EnemyData
{
    public string enemyName;
    public int level;
    public GameObject enemyPrefab;
    // Additional enemy stats here
}