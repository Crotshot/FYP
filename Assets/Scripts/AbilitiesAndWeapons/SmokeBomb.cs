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
                GameObject newProj = Instantiate(mortarShell, spawnPoint2);
                NetworkServer.Spawn(newProj);
                newProj.GetComponent<Team>().SetTeam(GetComponent<Team>().GetTeam());
                newProj.transform.parent = null;
                newProj.GetComponent<MortarProj>().SetTargetPos(Helpers.Vector3PointAlongLine(transform.position, GetComponent<Mouse_Pointer>().GetWorldFocal(), maxDistance));
            }
        }
    }

    private void Cast() {
        if (AbilityUsed()) {
            GameObject newProj = Instantiate(mortarShell, spawnPoint1);
            NetworkServer.Spawn(newProj);//Spawn mortar with mouse pos as target
            newProj.GetComponent<Team>().SetTeam(GetComponent<Team>().GetTeam());
            newProj.transform.parent = null;
            newProj.GetComponent<MortarProj>().SetTargetPos(Helpers.Vector3PointAlongLine(transform.position,GetComponent<Mouse_Pointer>().GetWorldFocal(), maxDistance));
            secondShotTimer = secondShotDelay;//Start timer delay
        }
    }
}
