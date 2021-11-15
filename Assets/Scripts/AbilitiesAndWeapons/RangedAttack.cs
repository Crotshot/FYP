using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class RangedAttack : NetworkBehaviour {
    [SerializeField] float roundsPerMinute = 60f;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject projectilePrefab;
    Inputs inputs;
    float fireRatetimer;
    int team;

    public UnityEvent weaponFired;

    void Start() {
        if (!hasAuthority)
            Destroy(this);

        inputs = FindObjectOfType<Inputs>();
        if (weaponFired == null)
            weaponFired = new UnityEvent();
        team = GetComponent<Team>().GetTeam();
    }

    void Update() {
        if (inputs.GetAttackInput() != 0 && fireRatetimer <= 0) {
            CmdSpawnBullet();
            fireRatetimer = 60f / roundsPerMinute;
            weaponFired?.Invoke();
        }

        if (fireRatetimer > 0) {
            fireRatetimer -= Time.deltaTime;
        }
    }

    public GameObject GetShellPrefab() {
        return projectilePrefab;
    }

    [Command]
    void CmdSpawnBullet() {
        GameObject newProj = Instantiate(projectilePrefab, spawnPoint);
        NetworkServer.Spawn(newProj);
        newProj.GetComponent<BasicShell>().Make(team);
        newProj.transform.parent = null;
    }

    public int GetTeam() {
        return team;
    }
}