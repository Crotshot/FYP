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
    [SerializeField] float minionSpeed, minionAngularSpeed, attackDistance;
    [SerializeField] Transform target;
    Vector3 destination;
    NavMeshAgent agentC;
    Minion_Attack attackC;

    public enum MinionState { Idle, OnPath, Follower, Forward, Defender, Fighting, Recalling, Retreating }
    MinionState minionState= MinionState.Idle;
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

    public void Setup()
    {
        if (isServer) {
            agentC = GetComponent<NavMeshAgent>();
            attackC = GetComponent<Minion_Attack>();
            agentC.speed = minionSpeed;
            agentC.angularSpeed = minionAngularSpeed;
            this.enabled = false;
        }
        else {
            Destroy(this);
        }
    }

    private void FixedUpdate()
    {
        if(target != null) {
            destination = target.position;
            agentC.destination = destination;
            if(Helpers.Vector3Distance(transform.position, destination) <= attackDistance) {
                //ADD checks to see if has line of sight
                //Rotation speed of minion so they dont snap turn
                agentC.destination = transform.position;
                transform.LookAt(destination);
                attackC.Attack();
            }
        }
    }


    public void SetDestination(Vector3 dest) {
        destination = dest;
        agentC.destination = destination;
    }

    public void SetTarget(Transform targ) {
        target = targ;
    }

    public string GetMinionType() {
        return minionType;
    }
}