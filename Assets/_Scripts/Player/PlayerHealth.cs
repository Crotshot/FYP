using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class PlayerHealth : Health { //Class for managin player health
    [SerializeField] float respawnTime;
    Vector3 spawnPoint;
    private float respawnTimer = 0;

    public void Start() {
        if (isServer)
            SetSpawnPoint();
        else
            CmdSetSpawnPoint();
    }

    [Command]
    public void CmdSetSpawnPoint() {
        SetSpawnPoint();
    }

    [ClientRpc]
    public void SetClientSpawnPoint(Vector3 point) {
        spawnPoint = point;
    }

    public void SetSpawnPoint() {
        GameObject[] points = GameObject.FindGameObjectsWithTag("Respawn");
        foreach (GameObject obj in points) {
            if (obj.GetComponent<Team>().GetTeam() == GetComponent<Team>().GetTeam()) {
                SetClientSpawnPoint(obj.transform.position);
                spawnPoint = obj.transform.position;
            }
        }
    }

    public override void Damage(float damage) {
        if (!dead) {
            base.Damage(damage);
            if (dead) {
                //LATER -> Kill all player minions
                RpcDeath();
            }
        }
    }

    [ClientRpc]
    public void RpcDeath() {
        if (TryGetComponent(out PlayerController pC))
            pC.enabled = false;
        if (TryGetComponent(out NavMeshAgent aI))
            aI.enabled = false;
        NavAgentEnabled(false);
        transform.position = spawnPoint + (Vector3.down * 30f);
        //if(hasAuthority)
        //    CmdSetPos(transform.position);
        //Invoke("RespawnPlayer", respawnTime);
        StartCoroutine("RespawnDelay");
    }

    [Command (requiresAuthority = false)]
    public void NavAgentEnabled(bool enable) {
        if (TryGetComponent(out NavMeshAgent aI))
            aI.enabled = enable;
    }
    //[Command]
    //private void CmdSetPos(Vector3 pos) {
    //    transform.position = pos;
    //}

    IEnumerator RespawnDelay() {
        respawnTimer = respawnTime;
        while (true) {
            yield return new WaitForSeconds(1.0f / 30.0f);
            respawnTimer -= 1.0f / 30.0f;

            if(respawnTimer <= 0) {
                break;
            }
        }
        RespawnPlayer();
    }

    public void RespawnPlayer() {
        transform.position = spawnPoint;
        //if (hasAuthority)
        //    CmdSetPos(transform.position);
        if (TryGetComponent(out NavMeshAgent aI))
            aI.enabled = true;
        NavAgentEnabled(true);
        if (TryGetComponent(out PlayerController pC))
            pC.enabled = true;
        ResetHealth();
    }

    public float GetRespawnTimer() {
        return respawnTimer;
    }

    public float GetRespawnTime() {
        return respawnTime;
    }
}