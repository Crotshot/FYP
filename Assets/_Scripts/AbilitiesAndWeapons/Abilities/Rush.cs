using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class Rush : Ability {
    /// <summary>
    /// Rush increases conqueror movespeed COMPLETE
    /// The conqueror can only move forward and cannot stop but can still turn
    /// Hitting another conqueror ends rush and stuns the second conqueror
    /// Hitting minions deals damage to the minions
    /// </summary>
    [SerializeField] float speedPercentIncrease, boostDuration, damage;
    [SerializeField] int stunTicks;
    [SerializeField] ParticleSystem rushEmmitter;
    [SerializeField] BoxCollider trigger;

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
                trigger.enabled = false;
                if (isServer) RpcEffect(false); else  CmdEffect(false);
            }
        }
    }

    private void Cast() {
        if (AbilityUsed()) {
            trigger.enabled = true;
            boostTimer = boostDuration;
            pC.SetCharacterSpeed(ogSpeed + ogSpeed * speedPercentIncrease);
            pC.EffectStart("Rush", 0);
            if (isServer) RpcEffect(true); else CmdEffect(true);
        }
    }

    [Command]
    public void CmdEffect(bool on) {
        RpcEffect(on);
    }

    [ClientRpc]
    public void RpcEffect(bool on) {
        var em = rushEmmitter.emission;
        em.enabled = on;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent(out Team team)) {
            if(team.GetTeam() != GetComponent<Team>().GetTeam()) {

                if(other.TryGetComponent(out Health health)) {
                    health.Damage(damage);
                }

                if (other.tag.Equals("Player")) {
                    other.GetComponent<Status>().AddEffect(Status.StatusEffect.Stun, stunTicks, 0);
                    boostTimer = 0;
                    pC.SetCharacterSpeed(ogSpeed);
                    pC.EffectEnd("Rush");
                    trigger.enabled = false;
                    if (isServer) RpcEffect(false); else CmdEffect(false);
                }
            }
        }
    }
}
