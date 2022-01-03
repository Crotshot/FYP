using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    Inputs inputs;
    NavMeshAgent agent;
    Transform navTarget;
    [SerializeField] bool offlineTest;

    private void Start() {
        if (hasAuthority || offlineTest) {
            navTarget = transform.GetChild(0).GetChild(1);
            //GetComponent<Respawn>().Setup();
            transform.GetChild(0).GetComponent<Camera_Follower>().Setup();
            inputs = FindObjectOfType<Inputs>();
            agent = GetComponent<NavMeshAgent>();
            GetComponent<Team>().AssignTeam();
        }
        else{
            Destroy(transform.GetChild(0).gameObject);
            Destroy(this);
        }
    }

    private void Update() {
        navTarget.localPosition = inputs.GetMovementInput();
        agent.destination = navTarget.position;
    }

    public bool getOfflineTest() {
        return offlineTest;
    }
}
////2D sprite mapped by the 3D object
//Inputs inputs;
//NavMeshAgent agent;
//Transform navTarget;
//[SerializeField] float agentSpeed, agentAngularSpeed;
//[SerializeField] int team = 0;

//private void Start()
//{
//    if (!isLocalPlayer) {
//        Destroy(transform.GetChild(0).gameObject);
//        Destroy(this);
//        return;
//    }
//    agent = GetComponent<NavMeshAgent>();
//    agent.speed = agentSpeed;
//    agent.angularSpeed = agentAngularSpeed;
//    
//    navTarget = transform.GetChild(0).GetChild(1);
//    inputs = FindObjectOfType<Inputs>();
//}

//public int GetTeam() {
//    return team;
//}