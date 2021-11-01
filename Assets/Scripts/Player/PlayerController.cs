using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    private void Start() {
        if (hasAuthority) {
            transform.GetChild(0).GetComponent<Camera_Follower>().Setup();
        }
        else{
            Destroy(transform.GetChild(0).gameObject);
            Destroy(this);
        }   
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

//private void Update()
//{
//    if (!isLocalPlayer) {
//        return;
//    }
//    navTarget.localPosition = inputs.GetMovementInput();
//    agent.destination = navTarget.position;
//}

//public int GetTeam() {
//    return team;
//}