using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LockIn : MonoBehaviour {
    public string conqName;
    public string[] minionNames;

    public void LockInSelection() {
        //Local Player has gameobject name set to Local inplayer constructor when its created
        GameObject.Find("Local").GetComponent<PlayerConstructor>().SetConqueror(conqName);
    }
}