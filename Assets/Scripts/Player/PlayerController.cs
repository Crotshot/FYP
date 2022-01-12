using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    Inputs inputs;
    NavMeshAgent agent;
    Transform navTarget;
    [SerializeField] bool offlineTest;

    public UnityEvent ab1, ab2, ab3;

    private void Start() {
        if (hasAuthority || offlineTest) {
            navTarget = transform.GetChild(0).GetChild(1);
            GetComponent<NavMeshAgent>().enabled = true;
            transform.GetChild(0).GetComponent<Camera_Follower>().Setup();
            inputs = FindObjectOfType<Inputs>();
            agent = GetComponent<NavMeshAgent>();

            if (ab1 == null)
                ab1 = new UnityEvent();
            if (ab2 == null)
                ab2 = new UnityEvent();
            if (ab3 == null)
                ab3 = new UnityEvent();
        }
        else{
            Destroy(transform.GetChild(0).gameObject);
            Destroy(this);
        }
    }

    private void Update() {
        navTarget.localPosition = inputs.GetMovementInput();
        if(agent.enabled)
            agent.destination = navTarget.position;

        if(inputs.GetAbility1Input() > 0) {
            ab1?.Invoke();
        }
        if (inputs.GetAbility2Input() > 0) {
            ab2?.Invoke();
        }
        if (inputs.GetAbility3Input() > 0) {
            ab3?.Invoke();
        }
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