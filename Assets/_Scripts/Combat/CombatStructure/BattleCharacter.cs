using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleCharacter
{
    // Basic character information
    public string characterName;
    public int level = 1;
    public bool isPlayerCharacter; // Determines if this is a player or enemy
    
    // Visual representation
    public GameObject characterPrefab; // The visual model
    public Sprite portrait; // For UI elements
    
    // Core Stats
    public int maxHP;
    public int currentHP;
    public int maxMP;
    public int currentMP;
    public int strength;
    public int defense;
    public int magic;
    public int magicDefense;
    public int speed;
    public int luck;
    
    // Keep track of original stats for buffs/debuffs
    private int baseStrength;
    private int baseDefense;
    private int baseMagic;
    private int baseMagicDefense;
    private int baseSpeed;
    private int baseLuck;
    
    // Echoes transformation state
    public bool isEchoesForm = false;
    public int echoesTurnsRemaining = 0;
    public int echoesLevel = 1;
    public float echoesEnergy = 0f; 
    public float maxEchoesEnergy = 100f; 
    
    // Addition system 
    public List<Addition> availableAdditions = new List<Addition>();
    public int currentAdditionIndex = 0;
    
    // Status effects
    public List<StatusEffect> statusEffects = new List<StatusEffect>();
    
    // Battle state tracking
    public bool hasTakenTurn = false;
    public Transform battlePosition; // Where this character stands in battle
    
    // Initialization
    public void Initialize()
    {
        // Store base stats for reference when buffs/debuffs are applied
        baseStrength = strength;
        baseDefense = defense;
        baseMagic = magic;
        baseMagicDefense = magicDefense;
        baseSpeed = speed;
        baseLuck = luck;
        
        // Set current HP/MP to max at initialization
        currentHP = maxHP;
        currentMP = maxMP;
    }
    
    // Health management
    public bool IsDead()
    {
        return currentHP <= 0;
    }
    
    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;
        
        // Add Echoes energy when taking damage (if player character)
        if (isPlayerCharacter && !isEchoesForm)
        {
            AddEchoesEnergy(amount * 0.2f);
        }
    }
    
    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
    }
    
    // MP management
    public bool UseMp(int amount)
    {
        if (currentMP >= amount)
        {
            currentMP -= amount;
            return true;
        }
        return false;
    }
    
    public void RestoreMp(int amount)
    {
        currentMP += amount;
        if (currentMP > maxMP) currentMP = maxMP;
    }
    
    // Echoes Form management
    public void AddEchoesEnergy(float amount)
    {
        if (!isEchoesForm) // Only add energy if not already transformed
        {
            echoesEnergy += amount;
            if (echoesEnergy > maxEchoesEnergy) echoesEnergy = maxEchoesEnergy;
        }
    }
    
    public bool CanTransform()
    {
        return isPlayerCharacter && !isEchoesForm && echoesEnergy >= maxEchoesEnergy;
    }
    
    public void TransformToEchoesForm()
    {
        if (CanTransform())
        {
            isEchoesForm = true;
            echoesTurnsRemaining = 3 + echoesLevel;
            echoesEnergy = 0;
            
            // Apply stat boosts
            float multiplier = 1.0f + (0.2f * echoesLevel);
            strength = Mathf.RoundToInt(baseStrength * multiplier);
            magic = Mathf.RoundToInt(baseMagic * multiplier);
            defense = Mathf.RoundToInt(baseDefense * multiplier);
            magicDefense = Mathf.RoundToInt(baseMagicDefense * multiplier);
            
            // Special effects or animations would be triggered here
        }
    }
    
    public void UpdateEchoesForm()
    {
        if (isEchoesForm)
        {
            echoesTurnsRemaining--;
            if (echoesTurnsRemaining <= 0)
            {
                RevertFromEchoesForm();
            }
        }
    }
    
    private void RevertFromEchoesForm()
    {
        isEchoesForm = false;
        
        // Revert stats to base values (accounting for active buffs/debuffs)
        ReapplyStatusEffects();
        
        // Special effects or animations would be triggered here
    }
    
    // Status effect management
    public void AddStatusEffect(StatusEffect effect)
    {
        // Check if this effect already exists
        StatusEffect existingEffect = statusEffects.Find(e => e.type == effect.type);
        
        if (existingEffect != null)
        {
            // Replace or stack effect?
            // Replace with the new one
            statusEffects.Remove(existingEffect);
            existingEffect.RemoveEffect(this);
        }
        
        // Add and apply the new effect
        StatusEffect newEffect = effect.CreateCopy();
        statusEffects.Add(newEffect);
        newEffect.ApplyEffect(this);
    }
    
    public void UpdateStatusEffects()
    {
        for (int i = statusEffects.Count - 1; i >= 0; i--)
        {
            if (statusEffects[i].UpdateEffect(this))
            {
                // Remove expired effects
                statusEffects[i].RemoveEffect(this);
                statusEffects.RemoveAt(i);
            }
        }
    }
    
    public void RemoveStatusEffect(StatusEffect.EffectType type)
    {
        StatusEffect effect = statusEffects.Find(e => e.type == type);
        
        if (effect != null)
        {
            effect.RemoveEffect(this);
            statusEffects.Remove(effect);
        }
    }
    
    public bool HasStatusEffect(StatusEffect.EffectType type)
    {
        return statusEffects.Exists(e => e.type == type);
    }
    
    public bool CanAct()
    {
        return !statusEffects.Exists(e => e.preventsAction);
    }
    
    // Reset stats and reapply all active status effects
    private void ReapplyStatusEffects()
    {
        // Reset stats to base values
        strength = baseStrength;
        defense = baseDefense;
        magic = baseMagic;
        magicDefense = baseMagicDefense;
        speed = baseSpeed;
        luck = baseLuck;
        
        // Reapply all active status effects
        foreach (var effect in statusEffects)
        {
            effect.ApplyEffect(this);
        }
    }
    
    // Calculate physical damage
    public int CalculatePhysicalDamage(int baseDamage, BattleCharacter target)
    {
        float damageMultiplier = (float)strength / (float)(target.defense + 1);
        int finalDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);
        return Mathf.Max(1, finalDamage); // Ensure at least 1 damage
    }
    
    // Calculate magical damage
    public int CalculateMagicalDamage(int baseDamage, BattleCharacter target)
    {
        float damageMultiplier = (float)magic / (float)(target.magicDefense + 1);
        int finalDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);
        return Mathf.Max(1, finalDamage); // Ensure at least 1 damage
    }
}

[System.Serializable]
public class Addition
{
    public string name;
    public string description;
    public int hitCount; // Number of timed hits
    public float baseDamageMultiplier = 1.0f; // Base damage without any successful hits
    public float maxDamageMultiplier = 2.5f; // Maximum damage with all successful hits
    
    // Mastery system
    public int currentMastery = 0;
    public int masteryRequired = 100; // How many successful completions to master
    public bool isMastered = false;
    
    // Timing windows for each hit
    public float[] timingWindows; // When to press button for each hit (0-1 range)
    public float successMargin = 0.15f; // How close to perfect timing counts as success
    
    // XP gained for Addition mastery when performing this Addition
    public int masteryXPForHit = 1;
    public int masteryXPForFullCombo = 10;
    
    // Special effects
    public string specialEffect; // e.g., "Stun", "Defense Down", etc.
    public float specialEffectChance = 0.0f; // Chance to apply special effect
    
    // Returns multiplier based on number of successful hits
    public float GetDamageMultiplier(int successfulHits)
    {
        if (hitCount == 0) return baseDamageMultiplier;
        
        float progressRatio = (float)successfulHits / hitCount;
        return Mathf.Lerp(baseDamageMultiplier, maxDamageMultiplier, progressRatio);
    }
    
    // Add mastery experience
    public void AddMasteryXP(int xp)
    {
        if (isMastered) return;
        
        currentMastery += xp;
        if (currentMastery >= masteryRequired)
        {
            isMastered = true;
            currentMastery = masteryRequired;
        }
    }
}

[System.Serializable]
public class StatusEffect
{
    public enum EffectType
    {
        Poison,
        Stun,
        Blind,
        Strength,
        Defense,
        Speed,
        Magic,
        MagicDefense,
        Luck,
        Custom
    }
    
    public string name;
    public string description;
    public EffectType type;
    public int turnsRemaining;
    public bool isPositive; // Whether this is a buff or debuff
    
    // Effect values
    public int flatValueChange = 0;       // For flat addition/subtraction effects
    public float percentValueChange = 0f; // For percentage-based effects
    
    // For damage-over-time effects like poison
    public int damagePerTurn = 0;
    
    // For status effects that prevent actions
    public bool preventsAction = false;
    
    // Visual effects
    public Sprite icon;
    public Color effectColor = Color.white;
    
    // Apply the effect to a character
    public void ApplyEffect(BattleCharacter target)
    {
        switch (type)
        {
            case EffectType.Poison:
                // Poison just deals damage over time, handled in UpdateEffect
                break;
                
            case EffectType.Stun:
                preventsAction = true;
                break;
                
            case EffectType.Blind:
                // Blind might reduce hit chance, implemented in battle system
                break;
                
            case EffectType.Strength:
                if (isPositive)
                    target.strength += flatValueChange;
                else
                    target.strength -= flatValueChange;
                // Ensure stat doesn't go below 1
                target.strength = Mathf.Max(1, target.strength);
                break;
                
            case EffectType.Defense:
                if (isPositive)
                    target.defense += flatValueChange;
                else
                    target.defense -= flatValueChange;
                target.defense = Mathf.Max(1, target.defense);
                break;
                
            case EffectType.Speed:
                if (isPositive)
                    target.speed += flatValueChange;
                else
                    target.speed -= flatValueChange;
                target.speed = Mathf.Max(1, target.speed);
                break;
                
            case EffectType.Magic:
                if (isPositive)
                    target.magic += flatValueChange;
                else
                    target.magic -= flatValueChange;
                target.magic = Mathf.Max(1, target.magic);
                break;
                
            case EffectType.MagicDefense:
                if (isPositive)
                    target.magicDefense += flatValueChange;
                else
                    target.magicDefense -= flatValueChange;
                target.magicDefense = Mathf.Max(1, target.magicDefense);
                break;
                
            case EffectType.Luck:
                if (isPositive)
                    target.luck += flatValueChange;
                else
                    target.luck -= flatValueChange;
                target.luck = Mathf.Max(1, target.luck);
                break;
                
            case EffectType.Custom:
                // Custom effects would be handled by game-specific logic
                break;
        }
    }
    
    // Update the effect each turn
    public bool UpdateEffect(BattleCharacter target)
    {
        // Apply ongoing effects
        if (type == EffectType.Poison)
        {
            target.TakeDamage(damagePerTurn);
        }
        
        // Reduce turns remaining
        turnsRemaining--;
        
        // Return true if effect should be removed
        return turnsRemaining <= 0;
    }
    
    // Remove the effect from a character
    public void RemoveEffect(BattleCharacter target)
    {
        // Undo stat changes
        switch (type)
        {
            case EffectType.Poison:
                // Nothing special needed for removal
                break;
                
            case EffectType.Stun:
                preventsAction = false;
                break;
                
            case EffectType.Strength:
                // We handle this in the character's ReapplyStatusEffects method
                break;
                
            case EffectType.Defense:
                // Same as above
                break;
                
            case EffectType.Speed:
                // Same as above
                break;
                
            case EffectType.Magic:
                // Same as above
                break;
                
            case EffectType.MagicDefense:
                // Same as above
                break;
                
            case EffectType.Luck:
                // Same as above
                break;
                
            case EffectType.Custom:
                // Custom cleanup logic
                break;
        }
    }
    
    // Create a copy of this effect (useful when applying from a template)
    public StatusEffect CreateCopy()
    {
        return new StatusEffect
        {
            name = this.name,
            description = this.description,
            type = this.type,
            turnsRemaining = this.turnsRemaining,
            isPositive = this.isPositive,
            flatValueChange = this.flatValueChange,
            percentValueChange = this.percentValueChange,
            damagePerTurn = this.damagePerTurn,
            preventsAction = this.preventsAction,
            icon = this.icon,
            effectColor = this.effectColor
        };
    }
}