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

    public UnityEvent weaponFired;

    void Start() {
        if (!hasAuthority  && !GetComponent<PlayerController>().getOfflineTest())
            Destroy(this);

        inputs = FindObjectOfType<Inputs>();
        if (weaponFired == null)
            weaponFired = new UnityEvent();
    }

    void Update() {
        if (inputs.GetAttackInput() != 0 && fireRatetimer <= 0) {
            if (GetComponent<PlayerController>().getOfflineTest()) {
                GameObject newProj = Instantiate(projectilePrefab, spawnPoint);
                newProj.transform.parent = null;
            }
            else
                Shoot();

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

    private void Shoot() {
        if (isServer) {
            GameObject newProj = Instantiate(projectilePrefab, spawnPoint);
            NetworkServer.Spawn(newProj);
            newProj.GetComponent<Team>().SetTeam(GetComponent<Team>().GetTeam());
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