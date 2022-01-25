using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class PlayerConstructor : NetworkBehaviour
{
    public CharacterStats conqueror;
    public CharacterStats[] minions;

    private void Start() {
        conqueror = new CharacterStats();
        minions = new CharacterStats[4];

        for(int i = 0; i < 4; i++) {
            minions[i] = new CharacterStats();
        }
    }

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        gameObject.name = "Local";
    }

    public void SetConqueror(CharacterStats stats) {        //Grab player prefab from GameManager
        conqueror = stats;
        CmdReadyUp(stats.name);
    }

    public void SetMinionNames(CharacterStats[] stats) {
        minions = stats;
    }

    [Command (requiresAuthority = false)]
    public void CmdCreateConqueror(NetworkConnectionToClient sender = null) {
        int teamNum = 0;
        if (connectionToClient.connectionId == 0) 
            teamNum = 1;
        else //When the server calls this it is 0 as there is no connection and any othe number can be team 2
            teamNum = 2;

        Vector3 pos = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        foreach (GameObject respawn in GameObject.FindGameObjectsWithTag("Respawn")) {
            if (respawn.GetComponent<Team>().GetTeam() == teamNum) {
                pos = respawn.transform.position;
                break;
            }
        }

        if(pos.z > 0) {
            rotation.eulerAngles = new Vector3(0, 180, 0);
        }

        GameObject playerSpawned = Instantiate(FindObjectOfType<GameManager>().GetConqueror(conqueror.name), pos, rotation);
        NetworkServer.Spawn(playerSpawned, sender);

        if (teamNum == 1) {
            playerSpawned.GetComponent<Team>().SetTeamColor(0.9f, 0.1f, 0.1f, 1);
        }
        else {
            playerSpawned.GetComponent<Team>().SetTeamColor(0.1f, 0.1f, 0.9f, 1);
        }

        if (connectionToClient.connectionId == 0) { //When the server calls this it is 0 as there is no connection and any othe number can be team 2
            playerSpawned.GetComponent<Team>().SetTeam(1);
            playerSpawned.GetComponent<Team>().SetTeamColor(0.9f,0.1f,0.1f,1);
            GetComponent<Team>().SetTeam(1);
            GetComponent<Team>().SetTeamColor(0.9f, 0.1f, 0.1f, 1);
        }
        else {
            playerSpawned.GetComponent<Team>().SetTeam(2);
            playerSpawned.GetComponent<Team>().SetTeamColor(0.1f, 0.1f, 0.9f, 1);
            GetComponent<Team>().SetTeam(2);
            GetComponent<Team>().SetTeamColor(0.1f, 0.1f, 0.9f, 1);
        }
    }


    [Command (requiresAuthority = false)]
    public void CmdReadyUp(string name) {
        conqueror.name = name;
        ((GameNetworkManager)NetworkManager.singleton).StartMatch();
    }
}