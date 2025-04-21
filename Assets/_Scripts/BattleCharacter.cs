[System.Serializable]
public class BattleCharacter
{
   public string name;
    public int level;
    
    // Stats
    public int maxHP;
    public int currentHP;
    public int maxMP;
    public int currentMP;
    public int strength;
    public int magic;
    public int defense;
    public int speed;
    
    // Battle state - updated terminology
    public bool isEchoesForm = false;
    public int echoesTurnsRemaining = 0;
    public int echoesLevel = 1;
    
    // For additions (timed hits)
    public List<Addition> availableAdditions = new List<Addition>();
    public int currentAdditionIndex = 0;
    
    // Status effects
    public List<StatusEffect> statusEffects = new List<StatusEffect>();
    
    // Character prefab reference
    public GameObject battlePrefab;
    
    // Returns true if character is defeated
    public bool IsDead()
    {
        return currentHP <= 0;
    }
    
    // Apply damage taking defense into account
    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(1, damage - defense/2);
        currentHP = Mathf.Max(0, currentHP - actualDamage);
    }

} 

[System.Serializable]
public class Addition
{
    public string name;
    public int hitCount;
    public float damageMultiplier;
    public float[] timingWindows; // When to press button for each hit
    public int masteryLevel = 0;  // How practiced the player is with this addition
}