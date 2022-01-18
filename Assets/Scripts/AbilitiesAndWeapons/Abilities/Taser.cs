using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using StaticHelpers = Crotty.Helpers.StaticHelpers;

public class Taser : Ability {
    [SerializeField] Transform spawnPoint;
    [SerializeField] float range, damage, stunTime;
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
            Debug.DrawRay(spawnPoint.position, spawnPoint.forward * range , Color.blue, 2.5f);
            Ray ray = new Ray(spawnPoint.position, spawnPoint.forward);
            float halfDist = range / 2.0f;
            if (Physics.Raycast(ray, out RaycastHit hit, range) && hit.collider.isTrigger == false) {
                Debug.Log("Taser hit Object: " + hit.collider.name + " at position" + hit.point);
                halfDist = StaticHelpers.Vector3Distance(hit.point, spawnPoint.position) / 2.0f;

                if (hit.collider.TryGetComponent(out Health health) && hit.collider.TryGetComponent(out Team team)) {
                    if(team.GetTeam() != GetComponent<Team>().GetTeam()) {
                        health.Damage(damage);
                    }
                }
            }
            TriggerEffect(halfDist);
        }
    }

    public void TriggerEffect(float dist) {
        if (isServer) {
            RpcEffect(dist);
            Effect(dist);
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

    [Command]
    private void CmdEffect(float dist) {
        TriggerEffect(dist);
    }

    [ClientRpc]
    private void RpcEffect(float dist) {
        Effect(dist);
    }
}