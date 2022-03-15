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
    //Transform navTarget;

    public UnityEvent ab1, ab2, ab3;
    private bool ready = false;

    public void Setup() {
        if (hasAuthority) {
            //navTarget = transform.GetChild(0).GetChild(1);
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

            Destroy(GetComponent<WorldSpaceHealthBar>());
            FindObjectOfType<UI>().Setup(GetComponent<PlayerHealth>(), GetComponent<PlayerCurrency>());
            GetComponent<Interactor>().Setup();
        }
        else{
            Destroy(transform.GetChild(0).gameObject);
            GetComponent<WorldSpaceHealthBar>().SetupDelayed();
            Destroy(this);
        }
    }

    private void Update() {
        if (!ready)
            return;
        //navTarget.localPosition = inputs.GetMovementInput();
        //if(agent.enabled && agent.isOnNavMesh)
        //    agent.destination = navTarget.position;

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


    public void Release() {
        ready = true;
    }
}