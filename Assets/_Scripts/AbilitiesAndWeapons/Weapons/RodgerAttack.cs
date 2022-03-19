using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class RodgerAttack : PlayerAttack {
    [SerializeField] float roundsPerMinute = 60f;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject projectilePrefab;
    float fireRatetimer;

    void Update() {
        if (fireRatetimer > 0) {
            fireRatetimer -= Time.deltaTime;
        }
    }

    override public void Attack() {
        Debug.Log("Shot fired!");
        if (fireRatetimer <= 0) {
            Shoot();
            fireRatetimer = 60f / roundsPerMinute;
            weaponFired?.Invoke();
        }
    }

    public GameObject GetShellPrefab() {
        return projectilePrefab;
    }

    private void Shoot() {
        if (isServer) {
            GameObject newProj = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
            NetworkServer.Spawn(newProj);
            newProj.transform.position = spawnPoint.position;
            newProj.transform.rotation = spawnPoint.rotation;
            newProj.GetComponent<Team>().SetTeam(GetComponent<Team>().GetTeam());
            newProj.GetComponent<BasicShell>().TeamAssigned();
            newProj.transform.parent = null;
        }
        else {
            CmdSpawnBullet();
        }
    }

    [Command]
    void CmdSpawnBullet() {
        Shoot();
    }
}
