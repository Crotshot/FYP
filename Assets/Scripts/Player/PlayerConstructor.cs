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

    public void SetConqueror(string name) {        //Grab player prefab from GameManager
        conquerorName = name;
        CmdReadyUp(name);
    }

    public void SetMinionNames(string[] names) {
        minionNames = names;
    }

    [Command (requiresAuthority = false)]
    public void CmdCreateConqueror(NetworkConnectionToClient sender = null) {
        GameObject playerSpawned = Instantiate(FindObjectOfType<GameManager>().GetConqueror(conquerorName), Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(playerSpawned, sender);

        if (connectionToClient.connectionId == 0) { //When the server calls this it is 0 as there is no connection and any othe number can be team 2
            playerSpawned.GetComponent<Team>().SetTeam(1);
            playerSpawned.GetComponent<Team>().SetTeamColor(0.9f,0.1f,0.1f,1);
        }
        else {
            playerSpawned.GetComponent<Team>().SetTeam(2);
            playerSpawned.GetComponent<Team>().SetTeamColor(0.1f, 0.1f, 0.9f, 1);
        }
    }

    [Command (requiresAuthority = false)]
    public void CmdReadyUp(string name) {
        conquerorName = name;
        ((GameNetworkManager)NetworkManager.singleton).StartMatch();
    }
}