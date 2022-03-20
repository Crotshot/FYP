using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//Minion Controller, Health, PlayerController
public class Status : NetworkBehaviour {
    //[SerializeField] For debugging in inspector
    [SerializeField] int stunTicks, speedBoostTicks, slownessTicks, burningTicks, weakenedTicks, abductionTicks, rootedTicks, magnetisedTicks, poisonTicks, regenTicks;
    [SerializeField] float speedBoostPerc, slownessPerc, burningDamage, weaknessPerc, magneticForce, poisonDamage, regenAmount;
    [SerializeField] Vector3 magneticForceOrigin;

    //Base Health Regen Percent is how much %hp a character will heal every second while alive
    [SerializeField] [Range(0, 1)] float baseHealthRegenPercent;
    [SerializeField] ParticleSystem burningEffect, stunEffect, poisonEffect;

    public enum StatusEffect {Stun, Speed, Slow, Burn, Weak, Abduct, Root, Magnet, Poison, Regen}

    Controller controller;
    Health health;
    bool init = false;
    private int tickRate;
    int baseRegenticks;
    float baseHealthRegen;

    private void Start() {
        health = GetComponent<Health>();
        controller = GetComponent<Controller>();
        CalcHealthRegen();
    }

    #region Setup
    public void Init() {
        init = true;
        StatusEffectManager SEM = FindObjectOfType<StatusEffectManager>();
        SEM.AddStatus(this);
        tickRate = SEM.GetTickRate();
        baseRegenticks = tickRate;
    }

    public void CalcHealthRegen() {
        baseHealthRegen = health.GetMaxHealth() * baseHealthRegenPercent;
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
        poisonTicks = 0;

        StatusUpdate();
    }
    #endregion

    public void StatusUpdate() {
        isBurning();
        isStunned();
        isPoisonned();
        HealthRegeneration();
    }
    
    #region Ticks
    [Server]
    private void HealthRegeneration() {
        if(baseRegenticks > 0) {
            baseRegenticks--;
        }
        else {
            baseRegenticks = tickRate;
            health.Heal(baseHealthRegen);
        }
    }

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

    [Server]
    public void isPoisonned() {
        if (poisonTicks == 0)
            return;

        if (poisonTicks == 1) {
            RpcEffect("PoisonEmitter", false);
            controller.EffectEnd("Poison");
        }

        poisonTicks--;
        health.Damage(poisonDamage);
    }
    #endregion

    #region AddingEffects
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
        else if (effectName == StatusEffect.Poison) {
            if (ticks < poisonTicks)
                return;
            RpcEffect("PoisonEmitter", true);
            poisonTicks = ticks;
            poisonDamage = damage;
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
        else if (emitterName.Equals("PoisonEmitter"))
            Effect(poisonEffect, on);
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