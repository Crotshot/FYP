using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
using Helpers = Crotty.Helpers.StaticHelpers;

public class MinionController : NetworkBehaviour
{
    [SerializeField] string minionType;
    [SerializeField] bool baseMinion;
    [SerializeField] float minionSpeed, minionAngularSpeed, attackDistance, awarnessDistance;
    [SerializeField] Transform attackTarget, assignedPoint;
    [SerializeField] List<Vector3> pathPoints = new List<Vector3>();
    Vector3 destination;
    NavMeshAgent agentC;
    Minion_Attack attackC;
    int layer;

    public enum MinionState { Idle, OnPath, Follower, Forward, Defender, Fighting, Recalling, Retreating }
    public MinionState minionState = MinionState.Idle, returningState = MinionState.Idle;
    /*
     Idle -> Default State, does nothing
     OnPath -> Minion is following a path, will fight enemies in way but will not hunt enemies off path
     Follower -> Following a conqeuror, is listening for Attack/Defend Commands and fights when their conqueror is fighting
     Forward -> Commanded State, minion is moving forward to attack or enter a building
     Defender -> Commanded State, Minion is staying in position and waiting for enemies to eneter a set range -> Minions will stay in area and not hunt
     Fighting -> In combat, if it is not a base minion it is listening for recall, retreat command -> on finish combat returns to previous state 
     Recalling -> Both State, Returning path/point/conqeuror, will fight anything in its way -> Listens to commands while returning
     Retreating -> Commanded State, Returning conqeuror, will ignore everything until with conqueror -> Turns to follower state after
     */
    //Minion Groups

    public void Setup() {
        agentC = GetComponent<NavMeshAgent>();
        attackC = GetComponent<Minion_Attack>();
        agentC.speed = minionSpeed;
        agentC.angularSpeed = minionAngularSpeed;
        pathPoints.Clear();
        layer = 1 << LayerMask.NameToLayer("Default");
        if (!isServer) {
            Destroy(this);
            return;
        }
        this.enabled = false;
    }
    
    RaycastHit[] hits;
    private void FixedUpdate() {
        Debug.DrawRay(transform.position, transform.forward * awarnessDistance, Color.white);
        if (minionState != MinionState.Fighting) {
            hits = Physics.SphereCastAll(transform.position, awarnessDistance, transform.forward, 0, layer, QueryTriggerInteraction.Ignore);
            if (hits.Length > 0) {
                foreach (RaycastHit hit in hits) {
                    if (hit.collider.TryGetComponent(out Team team) && hit.collider.TryGetComponent(out Health hp)) { //Get health cmpt as bullets and such have team comps
                        if(team.GetTeam() != GetComponent<Team>().GetTeam()) {
                            attackTarget = hit.transform;
                            returningState = minionState;
                            minionState = MinionState.Fighting;
                            agentC.speed = minionSpeed;
                            break;
                        }
                    }
                }
            }
        }
        else {
            if (Helpers.Vector3Distance(attackTarget.position, transform.position) > awarnessDistance * 1.1f) {
                attackTarget = null;
                minionState = returningState;
                if(minionState == MinionState.OnPath) {
                    agentC.speed = 8;
                }
                if (pathPoints.Count < 1) {
                    agentC.destination = assignedPoint.position + new Vector3(Random.Range(8f, -8f), 0, Random.Range(8f, -8f));
                }
                else {
                    agentC.destination = pathPoints[0];
                }
            }
        }

        if(attackTarget != null) {
            destination = attackTarget.position;
            agentC.destination = destination;
            if(Helpers.Vector3Distance(transform.position, destination) <= attackDistance) {
                //ADD checks to see if has line of sight
                //Rotation speed of minion so they dont snap turn
                agentC.destination = transform.position;
                transform.LookAt(destination);
                attackC.Attack();
            }
        }
        else {//Check distance to current destination //If less than 5m move onto next target
            if (pathPoints.Count > 0 && Helpers.Vector3Distance(transform.position, pathPoints[0]) <= 6f) {
                if(pathPoints.Count < 2) {//If that was the last target turn into to defence State
                    agentC.destination = pathPoints[0] + new Vector3( Random.Range(8f, -8f), 0, Random.Range(8f, -8f));
                    pathPoints.Remove(pathPoints[0]);
                    minionState = MinionState.Defender;
                    returningState = MinionState.Defender;
                    agentC.speed = minionSpeed;
                }
                else { //Else proceed to next path
                    pathPoints.Remove(pathPoints[0]); 
                    agentC.destination = pathPoints[0];
                }
            }
        }
    }

    public void AddPathPoints(Transform[] pathPointsToAdd) {
        foreach (Transform point in pathPointsToAdd) {
            pathPoints.Add(point.position);
        }
        agentC.destination = pathPoints[0];
        if(minionState == MinionState.Fighting) {
            returningState = MinionState.OnPath;
        }
        else {
            minionState = MinionState.OnPath;
            agentC.speed = 8;
        }
    }

    public void AssignControlPoint(Transform point) {
        if(assignedPoint != null) {
            assignedPoint.GetComponent<ControlPoint>().RemoveMinion(GetComponent<Team>().GetTeam());
        }
        assignedPoint = point;
        assignedPoint.GetComponent<ControlPoint>().AddMinion(GetComponent<Team>().GetTeam());
    }

    public Transform GetAssignedControlPoint() {
        return assignedPoint;
    }


    public void MinionDeath() {
        attackTarget = null;
        returningState = MinionState.Idle;
        minionState = MinionState.Idle;
        if (assignedPoint != null) {
            assignedPoint.GetComponent<ControlPoint>().RemoveMinion(GetComponent<Team>().GetTeam());
            assignedPoint = null;
        }
    }

    public void SetDestination(Vector3 dest) {
        destination = dest;
        agentC.destination = destination;
    }

    public void SetattackTarget(Transform targ) {
        attackTarget = targ;
    }

    public string GetMinionType() {
        return minionType;
    }

    public Transform GetMinionTarget() {
        return attackTarget;
    }

    public bool isBaseMinion() {
        return baseMinion;
    }
}