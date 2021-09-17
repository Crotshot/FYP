using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinionDetails : MonoBehaviour
{
    [SerializeField] private TMP_Text healthPoints, moveSpeed, physicalArmour, magicArmour, attackSpeed, attackDamage, minionName,
        rorM, description;
    [SerializeField] private Image minionArt;
    [SerializeField] private Sprite defaultBlackground;
    public bool assigned;
    public CharacterStats storedStats;

    public void AssignMinion(CharacterStats stats)
    {
        assigned = true;
        healthPoints.text = stats.healthPoints.ToString();
        moveSpeed.text = stats.movementSpeed.ToString();
        physicalArmour.text = stats.physicalArmour.ToString();
        magicArmour.text = stats.magicArmour.ToString();
        attackSpeed.text = stats.attackSpeed.ToString();
        attackDamage.text = stats.attackDamage.ToString();
        minionName.text = stats.name;
        if(stats.isRanged)
            rorM.text = "Ranged";
        else
            rorM.text = "Melee";
        description.text = stats.abilities[0].abilityDescription;
        minionArt.sprite = stats.characterArt;

        storedStats = stats;
    }

    public void XButtonPressed()
    {
        assigned = false;
        healthPoints.text = "-";
        moveSpeed.text = "-";
        physicalArmour.text = "-";
        magicArmour.text = "-";
        attackSpeed.text = "-";
        attackDamage.text = "-";
        minionName.text = "-";
        rorM.text = "-";
        description.text = "-";
        minionArt.sprite = defaultBlackground;

        FindObjectOfType<SelectionController>().MinionRemoved(storedStats); //Enable button for removed minion
        storedStats = null;


    }
}
