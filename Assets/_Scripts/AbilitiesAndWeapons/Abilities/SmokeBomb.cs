using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Helpers = Crotty.Helpers.StaticHelpers;

public class SmokeBomb : Ability {
    [SerializeField] GameObject mortarShell;
    [SerializeField] float secondShotDelay, maxDistance, shellSpeed = 60f, peakHeight = 60f, shellRadius = 0.5f;
    [SerializeField] Transform[] mortarBodies, gas, spawnPoints;
    [SerializeField] LayerMask layers, defLayer;
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
                mort.projBody.position += mort.projBody.forward * shellSpeed * Time.deltaTime;
                if(mort.projBody.position.y >= peakHeight) {
                    mort.state = MortarProjectile.ProjState.Down;
                    mort.projBody.position = new Vector3(mort.focal.x, peakHeight, mort.focal.z);
                    mort.projBody.transform.eulerAngles = new Vector3(90,0,0);
                }
            }
            else if (mort.state == MortarProjectile.ProjState.Down) {
                mort.projBody.position += mort.projBody.forward * shellSpeed * Time.deltaTime;
                Collider[] hits = Physics.OverlapSphere(mort.projBody.position, shellRadius, layers, QueryTriggerInteraction.Ignore);
                foreach (Collider hit in hits) {
                    if (hit.tag.Equals("minion") || hit.tag.Equals("Player")) {
                        if (hit.GetComponent<Team>().GetTeam() != GetComponent<Team>().GetTeam()) {
                            Contact(mort);
                            break;
                        }
                    }
                    else if (hit.gameObject.layer == defLayer) {
                        Contact(mort);
                        break;
                    }
                }

                if (mort.projBody.position.y <= 0) {
                    Contact(mort);
                }
            }
        }
    }

    private void Contact(MortarProjectile mort) {
        mort.gasCloud.Activate(mort.projBody.position);
        mort.projBody.position = Vector3.down * 50f;
        mort.state = MortarProjectile.ProjState.Inactive;
    }

    IEnumerator SecondShell() {
        yield return new WaitForSeconds(secondShotDelay);
        Fire();
        yield break;
    }

    private void Cast() {
        if (AbilityUsed()) {
            Fire();
            StartCoroutine("SecondShell");
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

        mortarProjectiles[projIndex].projBody.position = spawnPoints[projIndex].position;
        mortarProjectiles[projIndex].projBody.rotation = spawnPoints[projIndex].rotation;
        mortarProjectiles[projIndex].state = MortarProjectile.ProjState.Up;
        mortarProjectiles[projIndex].focal = focal;

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
    public Vector3 focal;
    public enum ProjState { Up, Down, Inactive }
    public ProjState state = ProjState.Inactive;
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