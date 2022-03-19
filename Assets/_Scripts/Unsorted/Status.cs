using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//Minion Controller, Health, PlayerController
public class Status : NetworkBehaviour {
    [SyncVar] int stunTicks, speedBoostTicks, slownessTicks, burningTicks, weakenedTicks, abductionTicks, rootedTicks, magnetisedTicks;
    [SyncVar] float speedBoostPerc, slownessPerc, burningDamage, weaknessPerc, magneticForce;
    [SyncVar] Vector3 forceOrigin;
    [SerializeField] ParticleSystem burningEffect, stunEffect;

    Controller controller;
    Health health;
    bool init = false;

    private void Start() {
        if (isServer) {
            health = GetComponent<Health>();
            controller = GetComponent<Controller>();
        }
    }

    #region Setup
    public void Init() {
        init = true;
        //Add from StatusEffectManager StatusList
    }

    public void DeInit() {
        init = false;
        Cleanse(false);
    }

    public bool GetInit() {
        return init;
    }

    //Remove all effects, called when character is defeated in combat, potentially by abilities also later on
    public void Cleanse(bool onlyNegative) {
        if (!onlyNegative) {
            speedBoostPerc = 0;
            speedBoostTicks = 0;
        }

        stunTicks = 0;
        slownessTicks = 0;
        burningTicks = 0;
        weakenedTicks = 0;
        abductionTicks = 0;
        rootedTicks = 0;
        magnetisedTicks = 0;
    }
    #endregion

    public void StatusUpdate() {
        isBurning();
        isStunned();
    }
    
    #region Ticks
    [Server]
    public void isBurning() {
        if (burningTicks == 0)
            return;

        if (burningTicks == 1) {
            RpcEffect("BurningEmitter", false);
        }

        burningTicks--;
        health.Damage(burningDamage);
    }

    [Server]
    public void isStunned() {
        if (stunTicks == 0)
            return;

        if (stunTicks == 1) {
            RpcEffect("StunEmitter", false);
            controller.EffectEnd("Stun");
        }

        stunTicks--;
    }
    #endregion

    #region AddingEffects
    [Server]
    public void Burn(float damage, int ticks) {
        if (ticks < burningTicks)
            return;

        RpcEffect("BurningEmitter", true); //Start particle emitter
        burningTicks = ticks;
        burningDamage = damage;
    }

    [Server]
    public void Stun(int ticks) {
        if (ticks < stunTicks)
            return;

        RpcEffect("StunEmitter", true); 
        stunTicks = ticks;
    }

    [Server]
    #endregion

    #region Effects
    [ClientRpc]
    public void RpcEffect(string emitterName, bool on) {
        if (emitterName.Equals("BurningEmitter"))
            Effect(burningEffect, on);
        else if (emitterName.Equals("StunEmitter"))
            Effect(stunEffect, on);
    }

    public void Effect(ParticleSystem pS, bool on) {
        var em = pS.emission;
        if (on) {
            em.enabled = true;
        }
        else {
            em.enabled = false;
        }
    }
    #endregion
}