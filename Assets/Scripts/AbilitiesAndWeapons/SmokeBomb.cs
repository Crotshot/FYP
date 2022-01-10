using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SmokeBomb : Ability {
    [SerializeField] GameObject mortarShell;
    [SerializeField] float secondShotDelay;
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
                Camera.main.ScreenToWorldPoint(Input.mousePosition);
                GameObject newProj = Instantiate(mortarShell, spawnPoint2);
                NetworkServer.Spawn(newProj);
                newProj.GetComponent<Team>().SetTeam(GetComponent<Team>().GetTeam());
                newProj.transform.parent = null;
            }
        }
    }

    private void Cast() {
        if (AbilityUsed()) {
            Camera.main.ScreenToWorldPoint(Input.mousePosition);//Get mouse position, MIGHT cause issues
            GameObject newProj = Instantiate(mortarShell, spawnPoint1);
            NetworkServer.Spawn(newProj);//Spawn mortar with mouse pos as target
            newProj.GetComponent<Team>().SetTeam(GetComponent<Team>().GetTeam());
            newProj.transform.parent = null;
            secondShotTimer = secondShotDelay;//Start timer delay
        }
    }
}
