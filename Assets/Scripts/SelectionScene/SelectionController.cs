using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionController : MonoBehaviour
{
    [SerializeField] GameObject characterCardPrefab, minionCardPrefab, characterSelectionContent, minionSelectionContent;
    private List<Button> characterButtons, minionButtons;
    [SerializeField] private TMP_Text healthPoints, moveSpeed, attackSpeed, attackDamage, characterName,
        pName, pDesc, ab1Name, ab1Desc, ab2Name, ab2Desc, sName, sDes;
    [SerializeField] private Image characterArt, pas, ab1, ab2, spec;
    [SerializeField] private MinionDetails[] selectedMinions;
    [SerializeField] private LockIn lockInScript;

    private void Start()
    {
        characterButtons = new List<Button>();
        minionButtons = new List<Button>();

        Object[] loadedCharacters = Resources.LoadAll("Selectable/Characters"); //Get all character stats from a folder
        Object[] loadedMinions = Resources.LoadAll("Selectable/Minions");

        CharacterStats[] loadedCharacterStats = new CharacterStats[loadedCharacters.Length];
        CharacterStats[] loadedMinionStats = new CharacterStats[loadedMinions.Length];

        for(int i = 0; i < loadedCharacters.Length; i++)
        {
            loadedCharacterStats[i] = (CharacterStats) loadedCharacters[i];
        }

        for (int i = 0; i < loadedMinions.Length; i++)
        {
            loadedMinionStats[i] = (CharacterStats)loadedMinions[i];
        }

        for (int i = 0; i <  loadedCharacterStats.Length; i++) //For every character stat
        {
            GameObject selecatableCard = Instantiate(characterCardPrefab); //Create a selectable card
            CardData data = selecatableCard.GetComponent<CardData>();
            data.art.sprite = loadedCharacterStats[i].characterArt; //And populate its data
            data.nameplate.text = loadedCharacterStats[i].name;
            selecatableCard.name = loadedCharacterStats[i].name; //Shows the name of character when obeserved in hierarchy
            data.stats = loadedCharacterStats[i];
            selecatableCard.transform.SetParent(characterSelectionContent.transform);//Child it to the content of the scroll view which auto moves it into place
            characterButtons.Add(selecatableCard.GetComponent<Button>());
        }

        for (int i = 0; i < loadedMinionStats.Length; i++) // And the same again for every minion
        {
            GameObject selecatableCard = Instantiate(minionCardPrefab);
            CardData data = selecatableCard.GetComponent<CardData>();
            data.art.sprite = loadedMinionStats[i].characterArt;
            data.nameplate.text = loadedMinionStats[i].name;
            selecatableCard.name = loadedMinionStats[i].name;
            data.stats = loadedMinionStats[i];
            selecatableCard.transform.SetParent(minionSelectionContent.transform);
            minionButtons.Add(selecatableCard.GetComponent<Button>());
        }   
    }

    public void CharacterButtonPressed(GameObject pressedButton)
    {
        foreach (Button button in characterButtons)
        {
            button.interactable = true;
            button.GetComponent<CardData>().disabledImage.enabled = false;
        }
        pressedButton.GetComponent<Button>().interactable = false;
        pressedButton.GetComponent<CardData>().disabledImage.enabled = true;

        CharacterStats stats = pressedButton.GetComponent<CardData>().stats;

        healthPoints.text = stats.healthPoints.ToString();
        moveSpeed.text = stats.movementSpeed.ToString();
        attackDamage.text = stats.attackDamage.ToString();
        attackSpeed.text = stats.attackSpeed.ToString();
        characterName.text = stats.name;
        lockInScript.conqName = stats.name;

        characterArt.sprite = stats.characterArt;

        pName.text = stats.abilities[0].name;
        pDesc.text = stats.abilities[0].abilityDescription;
        ab1Name.text = stats.abilities[1].name;
        ab1Desc.text = stats.abilities[1].abilityDescription;
        ab2Name.text = stats.abilities[2].name;
        ab2Desc.text = stats.abilities[2].abilityDescription;
        sName.text = stats.abilities[3].name;
        sDes.text = stats.abilities[3].abilityDescription;

        pas.sprite = stats.abilities[0].abilityArt;
        ab1.sprite = stats.abilities[1].abilityArt;
        ab2.sprite = stats.abilities[2].abilityArt;
        spec.sprite = stats.abilities[3].abilityArt;
    }

    public void MinionButtonPressed(GameObject pressedButton)
    {
        int validMinionIndex = 0;
        for(int i = 0; i < selectedMinions.Length; i++) //If all minions are assigned break out of the method
        {
            if (!selectedMinions[i].assigned)
            {
                validMinionIndex = i;
                break;
            }
            else
            {
                if(i == selectedMinions.Length - 1 && selectedMinions[i].assigned)
                {
                    return;
                }
            }
        }

        pressedButton.GetComponent<Button>().interactable = false;
        pressedButton.GetComponent<CardData>().disabledImage.enabled = true;

        selectedMinions[validMinionIndex].AssignMinion(pressedButton.GetComponent<CardData>().stats);
    }

    public void MinionRemoved(CharacterStats removedStats)
    {
        foreach (Button button in minionButtons)
        {
            if (button.GetComponent<CardData>().stats == removedStats)
            {
                button.interactable = true;
                button.GetComponent<CardData>().disabledImage.enabled = false;
            }
        }
    }
}