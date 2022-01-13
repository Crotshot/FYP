using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rush : Ability {

    [SerializeField] float speedPercentIncrease, boostDuration;
    float boostTimer, ogSpeed;

    private void Start() {
        if (!hasAuthority)
            Destroy(this); //This one does not need to exist outside of its owner

        SetUp(Cast);
        ogSpeed = GetComponent<NavMeshAgent>().speed;
    }

    private void FixedUpdate() {
        CoolDown(Time.deltaTime);

        if(boostTimer > 0) {
            //LATER -> Check destination and set it forward if not set
            boostTimer -= Time.deltaTime;

            if (boostTimer <= 0) {
                GetComponent<NavMeshAgent>().speed = ogSpeed;
            }
        }
    }

    private void Cast() {
        if (AbilityUsed()) {
            boostTimer = boostDuration;
            GetComponent<NavMeshAgent>().speed = ogSpeed + ogSpeed * speedPercentIncrease;
        }
    }
}
