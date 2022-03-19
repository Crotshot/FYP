using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rush : Ability {
    /// <summary>
    /// Rush increases conqueror movespeed COMPLETE
    /// The conqueror can only move forward and cannot stop but can still turn
    /// Hitting another conqueror ends rush and stuns the second conqueror
    /// Hitting minions deals damage to the minions
    /// </summary>
    [SerializeField] float speedPercentIncrease, boostDuration;
    [SerializeField] ParticleSystem rushEmmitter;
    float boostTimer, ogSpeed;
    PlayerController pC;

    private void Start() {
        if (!hasAuthority)
            Destroy(this); //This one does not need to exist outside of its owner
        pC = GetComponent<PlayerController>();
        SetUp(Cast);
        ogSpeed = pC.GetCharacterSpeed();
    }

    private void FixedUpdate() {
        CoolDown(Time.deltaTime);

        if(boostTimer > 0) {
            boostTimer -= Time.deltaTime;

            if (boostTimer <= 0) {
                pC.SetCharacterSpeed(ogSpeed);
                pC.EffectEnd("Rush");

                var em = rushEmmitter.emission;
                em.enabled = false;
            }
        }
    }

    private void Cast() {
        if (AbilityUsed()) {
            boostTimer = boostDuration;
            pC.SetCharacterSpeed(ogSpeed + ogSpeed * speedPercentIncrease);
            pC.EffectStart("Rush");

            var em = rushEmmitter.emission;
            em.enabled = true;
        }
    }
}
