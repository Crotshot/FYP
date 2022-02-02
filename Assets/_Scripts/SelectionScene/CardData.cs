using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardData : MonoBehaviour
{
    public Image art, disabledImage;
    public TMP_Text nameplate;
    public CharacterStats stats;
    public bool isMinion = false;

    public void ButtonPressed()
    {
        if (!isMinion)
            FindObjectOfType<SelectionController>().CharacterButtonPressed(gameObject);
        else
            FindObjectOfType<SelectionController>().MinionButtonPressed(gameObject);
    }
}
