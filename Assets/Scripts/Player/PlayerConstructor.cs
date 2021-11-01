using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerConstructor : NetworkBehaviour
{
    public string conquerorName;

    public string[] minionNames; //Needs to be moved

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        gameObject.name = "Local";
    }

    public void SetConqueror(string name) {
        //Grab player prefab from GameManager
        CmdReadyUp(name);
    }

    public void SetMinionNames(string[] names) {
        minionNames = names;
    }

    [Command (requiresAuthority = false)]
    public void CmdCreateConqueror(NetworkConnectionToClient sender = null) {
        GameObject playerSpawned = Instantiate(FindObjectOfType<GameManager>().GetConqueror(conquerorName), Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(playerSpawned, sender);
    }

    [Command (requiresAuthority = false)]
    public void CmdReadyUp(string name) {
        conquerorName = name;
        ((GameNetworkManager)NetworkManager.singleton).StartMatch();
    }
}