using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Helpers = Crotty.Helpers.StaticHelpers;

public class SmokeBomb : Ability {
    [SerializeField] GameObject mortarShell;
    [SerializeField] float secondShotDelay, maxDistance;
    [SerializeField] Transform[] mortarBodies, gas, spawnPoints;

    List<MortarProjectile> mortarProjectiles;
    int projIndex = 0;

    private void Awake() {
        mortarProjectiles = new List<MortarProjectile>();
        foreach (Transform mortarBody in mortarBodies) {
            mortarProjectiles.Add(new MortarProjectile(mortarBody, gas[projIndex].GetComponent<GasCloud>()));
            gas[projIndex].GetComponent<Team>().SetTeam(GetComponent<Team>().GetTeam());
            mortarBody.transform.parent = null;
            gas[projIndex].parent = null;
            projIndex++;
        }
    }

    private void Start() {
        SetUp(Cast);
    }

    private void FixedUpdate() {
        CoolDown(Time.deltaTime);
        foreach (MortarProjectile mort in mortarProjectiles) {
            if(mort.state == MortarProjectile.ProjState.Up) {

            }
            else if (mort.state == MortarProjectile.ProjState.Down) {

            }
        }
    }

    IEnumerator SecondShell() {
        yield return new WaitForSeconds(secondShotDelay);

        yield break;
    }

    private void Cast() {
        if (AbilityUsed()) {

        }
    }

    private void Fire() {
        Vector3 focal = GetComponent<Mouse_Pointer>().GetWorldFocal();
        if (isServer) {
            RpcFire(focal);
        }
        else {
            CmdFire(focal);
        }
    }

    [ClientRpc]
    private void RpcFire(Vector3 focal) {
        if (projIndex == mortarProjectiles.Count)
            projIndex = 0;



        projIndex++;
    }

    [Command]
    private void CmdFire(Vector3 focal) {
        RpcFire(focal);
    }
}

internal class MortarProjectile {
    public MortarProjectile(Transform body, GasCloud gc) {
        projBody = body;
        gasCloud = gc;
    }
    public Transform projBody;
    public GasCloud gasCloud;
    public enum ProjState { Up, Down, Inactive }
    public ProjState state = ProjState.Inactive;

    public ProjState Update() {
        return state;
    }
}
//if (secondShotTimer > 0) {
//    secondShotTimer -= Time.deltaTime;
//    if (secondShotTimer <= 0) {
//        if (isServer)
//            SpawnProj(spawnPoint2.position, spawnPoint2.eulerAngles, GetComponent<Mouse_Pointer>().GetWorldFocal());
//        else
//            CmdSpawnProj(spawnPoint2.position, spawnPoint2.eulerAngles, GetComponent<Mouse_Pointer>().GetWorldFocal());
//    }
//}

//if (isServer)
//    SpawnProj(spawnPoint1.position, spawnPoint1.eulerAngles, GetComponent<Mouse_Pointer>().GetWorldFocal());
//else
//    CmdSpawnProj(spawnPoint1.position, spawnPoint1.eulerAngles, GetComponent<Mouse_Pointer>().GetWorldFocal());
//secondShotTimer = secondShotDelay;//Start timer delay

//[Command]
//private void CmdSpawnProj(Vector3 pos, Vector3 rot, Vector3 focal) {
//    SpawnProj(pos, rot, focal);
//}

//private void SpawnProj(Vector3 pos, Vector3 rot, Vector3 focal) {
//    GameObject newProj = Instantiate(mortarShell, pos, Quaternion.Euler(rot));
//    NetworkServer.Spawn(newProj);//Spawn mortar with mouse pos as target
//    newProj.GetComponent<Team>().SetTeam(GetComponent<Team>().GetTeam());
//    newProj.transform.parent = null;
//    newProj.GetComponent<MortarProj>().SetTargetPos(Helpers.Vector3PointAlongLine(transform.position, focal, maxDistance));
//}