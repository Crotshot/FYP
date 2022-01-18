using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LockIn : MonoBehaviour {

    public CharacterStats conqStats;
    public CharacterStats[] minionStats;

    public void LockInSelection() { //Local Player has gameobject name set to Local in player constructor when its created
        PlayerConstructor player  = GameObject.Find("Local").GetComponent<PlayerConstructor>();
        player.SetConqueror(conqStats);
    }
}