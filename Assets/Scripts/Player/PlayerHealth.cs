using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class PlayerHealth : Health { //Class for managin player health
    [SerializeField] float respawnTime;
    Vector3 spawnPoint;

    public void Start() {
        if (hasAuthority) {
            GameObject[] points = GameObject.FindGameObjectsWithTag("Respawn");
            foreach (GameObject obj in points) {
                if (obj.GetComponent<Team>().GetTeam() == GetComponent<Team>().GetTeam()) {
                    spawnPoint = obj.transform.position;
                    break;
                }
            }
        }
    }

    public override void Damage(float damage) {
        base.Damage(damage);
        if (dead) {
            //LATER -> Kill all player minions
            RpcDeath();
        }
    }

    [ClientRpc]
    public void RpcDeath() {
        if (TryGetComponent(out NavMeshAgent aI))
            aI.enabled = false;
        if (TryGetComponent(out PlayerController pC))
            pC.enabled = false;
        transform.position = spawnPoint + Vector3.down * 20f;
        if(hasAuthority)
            CmdSetPos(transform.position);
        Invoke("RespawnPlayer", respawnTime);
    }

    [Command]
    private void CmdSetPos(Vector3 pos) {
        transform.position = pos;
    }

    public void RespawnPlayer() {
        if (TryGetComponent(out NavMeshAgent aI))
            aI.enabled = true;
        if (TryGetComponent(out PlayerController pC))
            pC.enabled = true;
        transform.position = spawnPoint + Vector3.down;
        if (hasAuthority)
            CmdSetPos(transform.position);
        ResetHealth();
    }
}