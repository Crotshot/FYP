using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability", order = 1)]
public class AbilityDescription : ScriptableObject
{
    public string abilityDescription;
    public Sprite abilityArt;
}
