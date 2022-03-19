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
    ParticleSystem pS;

    private void Start() {
        SetUp(Cast);
        pS = spawnPoint.GetComponent<ParticleSystem>();
    }

    private void FixedUpdate() {
        CoolDown(Time.deltaTime);
    }

    private void Cast() {
        if (AbilityUsed()) {
            //Raycast out of weapon & Get collision obsticle and point
            
            Ray ray = new Ray(spawnPoint.position, spawnPoint.forward);
            float halfDist = range / 2.0f;
            if (Physics.Raycast(ray, out RaycastHit hit, range, unitTerrainLayer, QueryTriggerInteraction.Ignore)) {
                halfDist = Helpers.Vector3Distance(hit.point, spawnPoint.position) / 2.0f;

#if UNITY_EDITOR
                Debug.Log("Taser hit Object: " + hit.collider.name + " at position" + hit.point);
                Debug.DrawRay(spawnPoint.position, spawnPoint.forward *  Helpers.Vector3Distance(hit.point, spawnPoint.position), Color.red, 5f);
                pos = hit.point;
#endif
                Collider[] scannedColliders = Physics.OverlapSphere(hit.point, radius, unitLayer, QueryTriggerInteraction.Ignore);
                for (int i = 0; i < scannedColliders.Length; i++) {
                    if (scannedColliders[i].TryGetComponent(out Health health) && scannedColliders[i].TryGetComponent(out Team team)) {
                        if (team.GetTeam() != GetComponent<Team>().GetTeam()) {
                            //Debug.Log("Damaging enemy");
                            health.Damage(damage);
                            health.GetComponent<Status>().AddEffect(Status.StatusEffect.Stun, stunTicks, 0);
                        }
                    }
                }
            }
            TriggerEffect(halfDist);
        }
    }

    public void TriggerEffect(float dist) {
        if (isServer) {
            RpcEffect(dist);
        }
        else {
            CmdEffect(dist);
        }
    }

    private void Effect(float dist) {
        var shape = pS.shape;
        shape.radius = dist;
        shape.position = new Vector3(0, 0, dist);
        pS.Play();
    }

    [Command (requiresAuthority = false)]
    private void CmdEffect(float dist) {
        TriggerEffect(dist);
    }

    [ClientRpc]
    private void RpcEffect(float dist) {
        Effect(dist);
    }


#if UNITY_EDITOR
    Vector3 pos = Vector3.zero;
    void OnDrawGizmosSelected() {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, radius);
    }
#endif
}