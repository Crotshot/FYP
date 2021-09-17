using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Character", order = 1)]
public class CharacterStats : ScriptableObject
{
    public Sprite characterArt;
    public int healthPoints, movementSpeed, physicalArmour, magicArmour, attackDamage;
    public float attackSpeed;
    public bool isRanged = false;
    /// <summary>
    /// For minions use only one ability with the name "N/A"
    /// </summary>
    public AbilityDescription[] abilities;
}