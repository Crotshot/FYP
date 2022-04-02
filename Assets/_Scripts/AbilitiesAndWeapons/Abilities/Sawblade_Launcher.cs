using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Sawblade_Launcher : Ability {
    [SerializeField] Transform sawblade, launchPoint;
    [SerializeField] float damage, slowStrength, radius, bladeSpeed, minY = 0.5f, gravity = 9.8f, scale = 2f;
    [SerializeField] LayerMask unitLayer, defaultLayer;
    [SerializeField] int slowTicks;
    [SerializeField] TrailRenderer tR;

    List<Health> trackedHealth;

    //                           0         1         2
    private enum BladeState { Inactive, Cooldown, Active }
    BladeState bState = BladeState.Inactive;

    private void Start() {
        sawblade.parent = null;
        if (!hasAuthority)
            return;
        SetUp(Cast);
        trackedHealth = new List<Health>();
    }

    private void FixedUpdate() {

        switch (bState) {
            case BladeState.Active:
                sawblade.GetChild(0).RotateAround(sawblade.position, sawblade.right, 360 * Time.deltaTime);
                sawblade.position += sawblade.forward * Time.deltaTime * bladeSpeed;
                if (sawblade.position.y > minY) {
                    sawblade.transform.position -= Vector3.up * Time.deltaTime * gravity;
                }

                if (hasAuthority) {
                    Collider[] cols = Physics.OverlapSphere(sawblade.position, radius, unitLayer, QueryTriggerInteraction.Ignore);
                    foreach (Collider other in cols) {//Check for Unit collision
                        if (other.TryGetComponent(out Health health)) {
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
                        bState = BladeState.Cooldown;            //Check for wall collision
                    }
                }
                break;
            case BladeState.Cooldown:
                if (hasAuthority && !CoolDown(Time.deltaTime)) {
                    ChangeState(0);
                }
                break;
            default:
                sawblade.position = launchPoint.position;
                sawblade.rotation = launchPoint.rotation;
                sawblade.GetChild(0).RotateAround(sawblade.position, sawblade.right, 360 * Time.deltaTime);
                break;
        }
                if (!hasAuthority)
            return;
    }

    private void Cast() {
        if (AbilityUsed()) {
            ChangeState(2);
            trackedHealth.Clear();
        }
    }

    #region Change State

    private void ChangeState(int n) {
        if (isServer) {
            RpcChangeState(n);
        }
        else {    //Options for changing state using ints or States, ints are quicker but States are more readable
            CmdChangeState(n);
        }
    }

    private void ChangeState(BladeState s) {
        int n = (int)s;
        if (isServer) {
            RpcChangeState(n);
        }
        else {
            CmdChangeState(n);
        }
    }

    [ClientRpc]
    private void RpcChangeState(int n) {
        bState = (BladeState)n;
        if (bState == BladeState.Active) {
            tR.emitting = true;
            sawblade.localScale = Vector3.one * scale;
        }
        else {
            tR.emitting = false;
            sawblade.localScale = Vector3.one;
        }
    }

    [Command]
    private void CmdChangeState(int n) {
        RpcChangeState(n);
    }
    #endregion
}
