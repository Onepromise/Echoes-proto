using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Character", menuName = "RPG/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public int level = 1;
    public bool isPlayerCharacter;
    
    // Stats
    public int maxHP = 100;
    public int maxMP = 50;
    public int strength = 10;
    public int defense = 10;
    public int magic = 10;
    public int magicDefense = 10;
    public int speed = 10;
    public int luck = 10;
    
    // Visual
    public GameObject characterPrefab;
    public Sprite portrait;
    
    // Create BattleCharacter instance from this data
    public BattleCharacter CreateBattleCharacter()
    {
        BattleCharacter battleChar = new BattleCharacter();
        
        // Copy basic info
        battleChar.characterName = characterName;
        battleChar.level = level;
        battleChar.isPlayerCharacter = isPlayerCharacter;
        
        // Copy stats
        battleChar.maxHP = maxHP;
        battleChar.currentHP = maxHP;
        battleChar.maxMP = maxMP;
        battleChar.currentMP = maxMP;
        battleChar.strength = strength;
        battleChar.defense = defense;
        battleChar.magic = magic;
        battleChar.magicDefense = magicDefense;
        battleChar.speed = speed;
        battleChar.luck = luck;
        
        // Copy visual elements
        battleChar.characterPrefab = characterPrefab;
        battleChar.portrait = portrait;
        
        // Initialize the character
        battleChar.Initialize();
        
        return battleChar;
    }
}