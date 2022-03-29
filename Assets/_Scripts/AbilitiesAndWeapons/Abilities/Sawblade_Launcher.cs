using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Sawblade_Launcher : Ability {
    [SerializeField] Transform sawblade, launchPoint;
    [SerializeField] float damage, slowStrength, radius, bladeSpeed, minY = 0.5f, gravity = 9.8f;
    [SerializeField] LayerMask unitLayer, defaultLayer;
    [SerializeField] int slowTicks;
    [SerializeField] TrailRenderer tR;

    List<Health> trackedHealth;
    bool active = true;

    private void Start() {
        SetUp(Cast);
        sawblade.parent = null;
        trackedHealth = new List<Health>();
    }

    private void FixedUpdate() {
        if (!CoolDown(Time.deltaTime)) {
            sawblade.position = launchPoint.position;
            sawblade.rotation = launchPoint.rotation;
            sawblade.GetChild(0).RotateAround(sawblade.position, sawblade.right, 360 * Time.deltaTime);
            Active(false);
        }

        if (active) {
            sawblade.GetChild(0).RotateAround(sawblade.position, sawblade.right, 360 * Time.deltaTime);
            sawblade.position += sawblade.forward * Time.deltaTime * bladeSpeed;
            if(sawblade.position.y > minY) {
                sawblade.transform.position -= Vector3.up * Time.deltaTime * gravity;
            }

            Collider[] cols = Physics.OverlapSphere(sawblade.position, radius, unitLayer, QueryTriggerInteraction.Ignore);
            foreach (Collider other in cols) {//Check for Unit collision
                if(other.TryGetComponent(out Health health)) {
                    if (trackedHealth.Contains(health))
                        continue;
                }
                if (other.tag.Equals("minion") || other.tag.Equals("Player")) {
                    if (other.GetComponent<Team>().GetTeam() != GetComponent<Team>().GetTeam()) {
                        other.GetComponent<Health>().Damage(damage);
                        trackedHealth.Add(other.GetComponent<Health>());
                        other.GetComponent<Status>().AddEffect(Status.StatusEffect.Slow, slowTicks, slowStrength);
                    }
                }
            }

            if (Physics.Raycast(sawblade.position, sawblade.forward, radius, defaultLayer, QueryTriggerInteraction.Ignore)) {
                Active(false);            //Check for wall collision
            }
        }
    }

    private void Cast() {
        if (AbilityUsed()) {
            Active(true);
            trackedHealth.Clear();
        }
    }


    private void Active(bool on) {
        active = on;

        if (isServer) {
            RpcActive(on);
        }
        else {
            CmdActive(on);
        }
    }


    [ClientRpc]
    private void RpcActive(bool on) {
        tR.emitting = on;
        if (on) {
            sawblade.GetChild(0).transform.localScale = Vector3.one * 3;
        }
        else {
            sawblade.GetChild(0).transform.localScale = Vector3.one;
        }
    }

    [Command]
    private void CmdActive(bool on) {
        RpcActive(on);
    }
}
