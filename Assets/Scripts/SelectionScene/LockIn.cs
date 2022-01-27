using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
public class LockIn : MonoBehaviour {

    public CharacterStats conqStats;
    public CharacterStats[] minionStats;

    [SerializeField] TMP_InputField inputF;
    PlayerConstructor player;

    private void Start() {
        player =  GameObject.Find("Local").GetComponent<PlayerConstructor>();
    }

    public void LockInSelection() { //Local Player has gameobject name set to Local in player constructor when its created
        player.SetConqueror(conqStats);
    }

    public void TextEdited() {
        if (inputF.text.Length == 0) {
            player.SetSeed(0);
            return;
        }
        int.TryParse(inputF.text, out int z);
        player.SetSeed(z);
    }
}