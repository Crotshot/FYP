using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class RangedAttack : NetworkBehaviour {
    [SerializeField] float roundsPerMinute = 60f;
    [SerializeField] Vector3 spawnPoint;
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
                GameObject newProj = Instantiate(projectilePrefab, transform.TransformPoint(spawnPoint), transform.rotation);
                newProj.transform.parent = null;
            }
            else
                CmdSpawnBullet(transform.TransformPoint(spawnPoint), transform.rotation); //Later update this to use the weapon.transform.rotation

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
    void CmdSpawnBullet(Vector3 pos, Quaternion rot, NetworkConnectionToClient sender = null) {
        GameObject newProj = Instantiate(projectilePrefab, pos, rot);
        NetworkServer.Spawn(newProj, sender);
       
        newProj.transform.parent = null;
    }
}