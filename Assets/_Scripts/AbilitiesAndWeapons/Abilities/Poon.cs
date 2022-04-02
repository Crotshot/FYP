using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Helpers = Crotty.Helpers.StaticHelpers;

public class Poon : Ability {
    //Min and max scale ability effect based on range, the ablity will not pull/stun anything within minDist and will pull/stun for max value outside maxDist
    [SerializeField] float damage, minAtDist, maxAtDist, pullRateMin, pullRateMax, travelSpeed, travelTime, poonRadius, force = 50f;
    [SerializeField] int activeTicksMax, activeTicksMin; //Min ticks at min Dist and max ticks at 80% range, anything within min dist does not pull
    [SerializeField] Transform poonPoint, poon;
    [SerializeField] LayerMask unitLayer, defaultLayer;

    List<Health> trackedHealth;
    int tickRate;
    

    //                          0,         1,        2,         3
    private enum PoonState { Inactive, Cooldown, Travelling, Pulling }
    PoonState pState = PoonState.Inactive;
    Transform stuckTarget;
    float timer;//Timer used for travell time and grapple time 

    private void Awake() { //Awake is called before server starts in scene so the child with a network identity "should" not cause trouble if detached in Awake()
        poon.parent = null;
    }

    private void Start() {
        if (hasAuthority) {
            SetUp(Cast);
            trackedHealth = new List<Health>();
        }
        tickRate = FindObjectOfType<StatusEffectManager>().GetTickRate();
    }

    private void Cast() {
        if (AbilityUsed()) {
            ChangeState(2);
            timer = travelTime;
            trackedHealth.Clear();
        }
    }


    private void FixedUpdate() {
        switch (pState) {
            case PoonState.Pulling: // Executes on both puller and pullee
                timer -= Time.deltaTime;
                if (timer <= 0) {
                    if(hasAuthority)
                        ChangeState(1);
                    stuckTarget = null;
                    break;
                }

                if (!hasAuthority && stuckTarget != null) {
                    stuckTarget.LookAt(transform.position);

                    float temp = Helpers.Vector3Distance(poonPoint.position, stuckTarget.position);
                    if (temp < minAtDist) {
                        break;
                    }
                    temp = Mathf.Clamp((temp - minAtDist) / (maxAtDist - minAtDist), 0, 1);
                    temp = (pullRateMin * (1f - temp) + (pullRateMax * temp));

                    stuckTarget.GetComponent<Rigidbody>().velocity = stuckTarget.forward * Time.deltaTime * temp * force; //For some reason cannot edit the velocity on the Conqueror
                    //Debug.Log("Velocity: " + stuckTarget.GetComponent<Rigidbody>().velocity + ", position x: " + stuckTarget.transform.position.x + ", y: " + stuckTarget.transform.position.y + ", z: " + stuckTarget.transform.position.z);
                }
                break;

            case PoonState.Travelling:
                poon.position += poon.forward * travelSpeed * Time.deltaTime;
                if (!hasAuthority)
                    break;

                timer -= Time.deltaTime;
                if (timer <= 0) {
                    ChangeState(1);
                }
                else if (Physics.Raycast(poon.position, poon.forward, 1f, defaultLayer, QueryTriggerInteraction.Ignore)) {
                    ChangeState(1);         //Check for wall collision
                }

                Collider[] cols = Physics.OverlapSphere(poon.position, poonRadius, unitLayer, QueryTriggerInteraction.Ignore);
                foreach (Collider other in cols) {//Check for Unit collision
                    if (other.TryGetComponent(out Team team)) {
                        if (team.GetTeam() != GetComponent<Team>().GetTeam()) { //Right now works on minions, infuture will only work on Conquerors
                            Health hp = team.GetComponent<Health>();
                            if (!trackedHealth.Contains(hp)) {
                                trackedHealth.Add(hp);
                                hp.Damage(damage);
                            }
                            
                            if (team.tag.Equals("Player")) {
                                uint id = other.GetComponent<NetworkIdentity>().netId;
                                stuckTarget = other.transform;


                                float percDist = Helpers.Vector3Distance(poonPoint.position, stuckTarget.position);
                                if (percDist < minAtDist) {
                                    ChangeState(1);
                                    break;
                                }

                                //ChangeState(3); ->> Not needed moved to AssignTarget

                                percDist = Mathf.Clamp((percDist - minAtDist) / (maxAtDist - minAtDist), 0, 1); //Percent Distance
                                int tempTicks = (int)(activeTicksMin * (1f - percDist) + activeTicksMax * (percDist));
                                timer = ((float)tempTicks) / ((float)tickRate);

                                AssignTarget(id, timer);
                                team.GetComponent<Status>().AddEffect(Status.StatusEffect.Stun, tempTicks, 0);
                                break;
                            }
                        }
                    }
                }
                break;

            case PoonState.Cooldown:
                if (hasAuthority && !CoolDown(Time.deltaTime)) {
                    ChangeState(0);
                }
                break;

            default:
                poon.position = poonPoint.position;
                poon.rotation = poonPoint.rotation;
                break;
        }
    }

    #region AssignTarget
    private void AssignTarget(uint n, float t) {
        if (isServer) {
            RpcAssignTarget(n, t);
        }
        else {
            CmdAssignTarget(n, t);
        }
    }

    [ClientRpc]
    private void RpcAssignTarget(uint n, float t) {
        pState = PoonState.Pulling;
        foreach (NetworkIdentity id in FindObjectsOfType<NetworkIdentity>()) {
            if(id.netId == n) { //Only executes on the client who has authority over the specific character
                if (id.TryGetComponent(out PlayerController pc)) {
                    if (pc.AuthCheck()) {
                        timer = t;
                        stuckTarget = id.transform;
                        break;
                    }
                }
            }
        }
    }

    [Command]
    private void CmdAssignTarget(uint n, float t) {
        RpcAssignTarget(n, t);
    }
    #endregion

    #region Change State
    //Options for changing state using ints or States, ints are quicker but States are more readable
    private void ChangeState(int n) {
        if (isServer) {
            RpcChangeState(n);
        }
        else {
            CmdChangeState(n);
        }
    }

    private void ChangeState(PoonState s) {
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
        pState = (PoonState)n;
    }

    [Command]
    private void CmdChangeState(int n) {
        RpcChangeState(n);
    }
    #endregion


#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(poon.position, poonRadius);
    }
#endif
}
//Code graveyard for 1/2 finished prototype
/*
    private void Awake() { //Awake is called before server starts in scene so the child with a network identity "should" not cause trouble if detached in Awake()
        poon.parent = null;    
    }

    private void Start() {
        if (!hasAuthority)
            return;
        SetUp(Cast);
        tickRate = FindObjectOfType<StatusEffectManager>().GetTickRate();
    }

    private void FixedUpdate() {
        if (!hasAuthority)
            return;

        switch (pState) {
            case PoonState.Pulling:

                timer -= Time.deltaTime;
                if (timer <= 0) {
                    pState = PoonState.Inactive;
                    stuckTarget = null;
                    return;
                }

                stuckTarget.LookAt(transform.position);

                float temp = Helpers.Vector3Distance(poonPoint.position, stuckTarget.position);
                if (temp < minAtDist) {
                    return;
                }

                temp = Mathf.Clamp((temp - minAtDist) / (maxAtDist - minAtDist), 0, 1);
                temp = (pullRateMin * (1f - temp) + pullRateMax * (temp));
                stuckTarget.GetComponent<Rigidbody>().velocity = stuckTarget.forward * Time.deltaTime * temp;
                break;


            case PoonState.Travelling:
                poon.position += poon.forward * travelSpeed * Time.deltaTime;
                timer -= Time.deltaTime;
                if (timer <= 0) {
                    pState = PoonState.Inactive;
                }
                else if (Physics.Raycast(poon.position, poon.forward, 1f, defaultLayer, QueryTriggerInteraction.Ignore)) {
                    pState = PoonState.Inactive;          //Check for wall collision
                }
                else if (Physics.Raycast(poon.position, poon.forward, out RaycastHit hit, 1f, unitLayer, QueryTriggerInteraction.Ignore)) {
                    if (hit.collider.TryGetComponent(out Team team)) {
                        if (team.GetTeam() != GetComponent<Team>().GetTeam()) { //Right now works on minions, infuture will only work on Conquerors
                            team.GetComponent<Health>().Damage(damage);
                            if (team.tag.Equals("Player")) {
                                stuckTarget = hit.transform;
                                //poon.parent = stuckTarget;

                                pState = PoonState.Pulling;
                                float percDist = Helpers.Vector3Distance(poonPoint.position, hit.point);
                                if (percDist < minAtDist) {
                                    pState = PoonState.Inactive;
                                    return;
                                }
                                percDist = Mathf.Clamp((percDist - minAtDist) / (maxAtDist - minAtDist), 0, 1); //Percent Distance
                                int tempTicks = (int)(activeTicksMin * (1f - percDist) + activeTicksMax * (percDist));
                                timer = ((float)tempTicks) / ((float)tickRate);

                                team.GetComponent<Status>().AddEffect(Status.StatusEffect.Stun, tempTicks, 0);
                            }
                        }
                    }
                }
                break;
            default:
                if (!CoolDown(Time.deltaTime)) {
                    poon.position = poonPoint.position;
                    poon.rotation = poonPoint.rotation;
                }
                break;
        }
    }

    private void Cast() {
        if (AbilityUsed()) {
            pState = PoonState.Travelling;
            timer = travelTime;
        }
    }
*/
