using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class PlayerHealth : Health { //Class for managin player health
    [SerializeField] protected float respawnTime;
    [SerializeField] protected ParticleSystem deathFire;
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
                RpcDeath();
            }
        }
    }

    [ClientRpc]
    public void RpcDeath() {
        if (TryGetComponent(out PlayerController pC))
            pC.enabled = false; //All chracter abilities and their weapon is disabled due to the controller being disabled
        if (TryGetComponent(out MeshCollider mC))
            mC.enabled = false;
        if (TryGetComponent(out Rigidbody rB))
            rB.useGravity = false;

        transform.position -= Vector3.up * 0.5f;
        var em = deathFire.emission;
        em.enabled = true;
        StartCoroutine("RespawnDelay");
    }

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

        if (TryGetComponent(out PlayerController pC))
            pC.enabled = true;
        if (TryGetComponent(out MeshCollider mC))
            mC.enabled = true;
        if (TryGetComponent(out Rigidbody rB))
            rB.useGravity = true;

        var em = deathFire.emission;
        em.enabled = false;

        ResetHealth();
    }

    public float GetRespawnTimer() {
        return respawnTimer;
    }

    public float GetRespawnTime() {
        return respawnTime;
    }
}