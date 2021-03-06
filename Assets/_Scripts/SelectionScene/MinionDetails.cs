using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinionDetails : MonoBehaviour
{
    [SerializeField] private TMP_Text healthPoints, moveSpeed, attackSpeed, attackDamage, minionName, description, cost;
    [SerializeField] private Image minionArt;
    [SerializeField] private Sprite defaultBlackground;
    public bool assigned;
    public CharacterStats storedStats;

    public void AssignMinion(CharacterStats stats)
    {
        assigned = true;
        healthPoints.text = stats.healthPoints.ToString();
        moveSpeed.text = stats.movementSpeed.ToString();
        attackDamage.text = stats.attackDamage.ToString();
        attackSpeed.text = stats.attackSpeed.ToString();
        minionName.text = stats.name;
        cost.text = stats.cost.ToString();

        description.text = stats.abilities[0].abilityDescription;
        minionArt.sprite = stats.characterArt;

        storedStats = stats;
    }

    public void XButtonPressed()
    {
        assigned = false;
        healthPoints.text = "-";
        moveSpeed.text = "-";
        attackSpeed.text = "-";
        attackDamage.text = "-";
        minionName.text = "-";
        description.text = "-";
        cost.text = "-";
        minionArt.sprite = defaultBlackground;

        FindObjectOfType<SelectionController>().MinionRemoved(storedStats); //Enable button for removed minion
        storedStats = null;


    }
}
