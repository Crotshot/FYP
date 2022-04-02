using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerBuilder : MonoBehaviour
{
    private void Start() {
        GameObject localPlayer = GameObject.Find("Local");
        if (localPlayer == null)
            return;

        localPlayer.GetComponent<PlayerConstructor>().CmdCreateConqueror();
    }
}