using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//Minion Controller, Health, PlayerController
public class Status : NetworkBehaviour {
    /*[SyncVar] */int stunTicks, speedBoostTicks, slownessTicks, burningTicks, weakenedTicks, abductionTicks, rootedTicks, magnetisedTicks;
    /*[SyncVar] */float speedBoostPerc, slownessPerc, burningDamage, weaknessPerc, magneticForce;
    /*[SyncVar] */Vector3 magneticForceOrigin;
    [SerializeField] ParticleSystem burningEffect, stunEffect;

    public enum StatusEffect {Stun, Speed, Slow, Burn, Weak, Abduct, Root, Magnet}

    Controller controller;
    Health health;
    bool init = false;

    private void Start() {
        health = GetComponent<Health>();
        controller = GetComponent<Controller>();
    }

    #region Setup
    public void Init() {
        init = true;
        FindObjectOfType<StatusEffectManager>().AddStatus(this);
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
            speedBoostTicks = 0;
        }

        stunTicks = 0;
        slownessTicks = 0;
        burningTicks = 0;
        weakenedTicks = 0;
        abductionTicks = 0;
        rootedTicks = 0;
        magnetisedTicks = 0;

        StatusUpdate();
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
            controller.EffectEnd("Burn");
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
    //[Server]
    //public void Burn(float damage, int ticks) {
    //    if (ticks < burningTicks)
    //        return;

    //    RpcEffect("BurningEmitter", true); //Start particle emitter
    //    burningTicks = ticks;
    //    burningDamage = damage;
    //}

    //[Server]
    //public void Stun(int ticks) {
    //    if (ticks < stunTicks)
    //        return;

    //    RpcEffect("StunEmitter", true);
    //    controller.EffectStart("Stun");
    //    stunTicks = ticks;
    //}

    /// <summary>
    /// Generic Status effect function that can apply any status effect
    /// </summary>
    /// <param name="effectName"></param>
    /// <param name="ticks"></param>
    /// <param name="damage"></param>
    /// 
    public void AddEffect(StatusEffect effectName, int ticks, float damage) {
        if (!isServer) {
            CmdAddEffect(effectName, ticks, damage);
            return;
        }
        if (effectName == StatusEffect.Burn) {
            if (ticks < burningTicks)
                return;

            RpcEffect("BurningEmitter", true); //Start particle emitter
            burningTicks = ticks;
            burningDamage = damage;
        }
        else if (effectName == StatusEffect.Stun) {
            if (ticks < stunTicks)
                return;
            controller.EffectStart("Stun");

            RpcEffect("StunEmitter", true);
            stunTicks = ticks;
        }
    }

    [Command (requiresAuthority = false)]
    public void CmdAddEffect(StatusEffect effectName, int ticks, float damage) {
        AddEffect(effectName, ticks, damage);
    }
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