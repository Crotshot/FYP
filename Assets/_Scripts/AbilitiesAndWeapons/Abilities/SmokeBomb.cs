using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Helpers = Crotty.Helpers.StaticHelpers;

public class SmokeBomb : Ability {
    [SerializeField] GameObject mortarShell;
    [SerializeField] float secondShotDelay, maxDistance;
    [SerializeField] Transform spawnPoint1, spawnPoint2;
    float secondShotTimer;

    private void Start() {
        SetUp(Cast);
    }

    private void FixedUpdate() {
        CoolDown(Time.deltaTime);

        if (secondShotTimer > 0) {
            secondShotTimer -= Time.deltaTime;
            if (secondShotTimer <= 0) {
                if (isServer)
                    SpawnProj(spawnPoint2.position, spawnPoint2.eulerAngles, GetComponent<Mouse_Pointer>().GetWorldFocal());
                else
                    CmdSpawnProj(spawnPoint2.position, spawnPoint2.eulerAngles, GetComponent<Mouse_Pointer>().GetWorldFocal());
            }
        }
    }

    private void Cast() {
        if (AbilityUsed()) {
            if (isServer)
                SpawnProj(spawnPoint1.position, spawnPoint1.eulerAngles, GetComponent<Mouse_Pointer>().GetWorldFocal());
            else
                CmdSpawnProj(spawnPoint1.position, spawnPoint1.eulerAngles, GetComponent<Mouse_Pointer>().GetWorldFocal());
            secondShotTimer = secondShotDelay;//Start timer delay
        }
    }

    [Command]
    private void CmdSpawnProj(Vector3 pos, Vector3 rot, Vector3 focal) {
        SpawnProj(pos, rot, focal);
    }

    private void SpawnProj(Vector3 pos, Vector3 rot, Vector3 focal) {
        GameObject newProj = Instantiate(mortarShell, pos, Quaternion.Euler(rot));
        NetworkServer.Spawn(newProj);//Spawn mortar with mouse pos as target
        newProj.GetComponent<Team>().SetTeam(GetComponent<Team>().GetTeam());
        newProj.transform.parent = null;
        newProj.GetComponent<MortarProj>().SetTargetPos(Helpers.Vector3PointAlongLine(transform.position, focal, maxDistance));
    }
}
