using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Helpers = Crotty.Helpers.StaticHelpers;

public class Taser : Ability {
    [SerializeField] Transform spawnPoint;
    [SerializeField] float range, damage, radius = 2f;
    [SerializeField] LayerMask unitLayer, unitTerrainLayer;
    [SerializeField] int stunTicks;
    [SerializeField] ParticleSystem aoe;
    ParticleSystem pS;

    private void Start() {
        if(hasAuthority)
            SetUp(Cast);
        pS = spawnPoint.GetComponent<ParticleSystem>();
        aoe.transform.parent = null;
        var shape = aoe.shape;
        shape.radius = radius;
    }

    private void FixedUpdate() {
        if(hasAuthority)
            CoolDown(Time.deltaTime);
    }

    private void Cast() {
        if (AbilityUsed()) {
            Ray ray = new Ray(spawnPoint.position, spawnPoint.forward);//Raycast out of weapon & Get collision obsticle and point
            float halfDist = range / 2.0f;
            if (Physics.Raycast(ray, out RaycastHit hit, range, unitTerrainLayer, QueryTriggerInteraction.Ignore)) {
                halfDist = Helpers.Vector3Distance(hit.point, spawnPoint.position) / 2.0f;

#if UNITY_EDITOR
                Debug.Log("Taser hit Object: " + hit.collider.name + " at position" + hit.point);
                Debug.DrawRay(spawnPoint.position, spawnPoint.forward *  Helpers.Vector3Distance(hit.point, spawnPoint.position), Color.red, 5f);
                pos = hit.point;
#endif

                Collider[] scannedColliders = Physics.OverlapSphere(hit.point, radius, unitLayer, QueryTriggerInteraction.Collide);
                for (int i = 0; i < scannedColliders.Length; i++) {
                    if(scannedColliders[i].TryGetComponent(out Team team))
                        if (scannedColliders[i].TryGetComponent(out Health health)) {
                            if (team.GetTeam() != GetComponent<Team>().GetTeam()) {
                                //Debug.Log("Damaging enemy");
                                health.Damage(damage);
                                health.GetComponent<Status>().AddEffect(Status.StatusEffect.Stun, stunTicks, 0);
                            }
                        }
                        //else if(scannedColliders[i].TryGetComponent(out GasCloud gas)) {
                        //    gas.TaserStun(stunTicks);
                        //}
                }

                //For some reason the gas clouds does not get scanned in the overlapspere so I am manually checking this here
                foreach (GasCloud cloud in FindObjectsOfType<GasCloud>()) {
                    //Debug.Log("Dist: " + Helpers.Vector3Distance(hit.point, cloud.transform.position) + ", max dist: " + (radius + cloud.transform.localScale.x * 1.2f));
                    if (Helpers.Vector3Distance(hit.point, cloud.transform.position) <= radius + cloud.transform.localScale.x * 1.2f) {//cloud.transform.localScale.x is radius of cloud & 1.2 is og radius of sphere it scales
                        if (cloud.GetComponent<Team>().GetTeam() == GetComponent<Team>().GetTeam()) {
                            cloud.TaserStun(stunTicks);
                        }
                    }
                }
            }

            if (isServer) {
                RpcEffect(halfDist, hit.point);
            }
            else {
                CmdEffect(halfDist, hit.point);
            }
        }
    }

    [Command (requiresAuthority = false)]
    private void CmdEffect(float dist, Vector3 pos) {
        RpcEffect(dist, pos);
    }

    [ClientRpc]
    private void RpcEffect(float dist, Vector3 pos) {
        aoe.transform.position = pos;
        aoe.Play();

        var shape = pS.shape;
        shape.radius = dist;
        shape.position = new Vector3(0, 0, dist);
        pS.Play();
    }


#if UNITY_EDITOR
    Vector3 pos = Vector3.zero;
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;        // Draw a yellow sphere at the transform's position
        Gizmos.DrawWireSphere(pos, radius);
    }
#endif
}