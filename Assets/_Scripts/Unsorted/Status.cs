using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//Minion Controller, Health, PlayerController
public class Status : NetworkBehaviour {
    //[SerializeField] For debugging in inspector
    [SerializeField] int stunTicks, speedTicks, slownessTicks, burningTicks, weakenedTicks, abductionTicks, rootedTicks, magnetisedTicks, poisonTicks, regenTicks;
    [SerializeField] float speedPerc, slownessPerc, burningDamage, weaknessPerc, magneticForce, poisonDamage, regenAmount;
    [SerializeField] Vector3 magneticForceOrigin;

    //Base Health Regen Percent is how much %hp a character will heal every second while alive
    [SerializeField] [Range(0, 1)] float baseHealthRegenPercent;
    [SerializeField] ParticleSystem burningEffect, stunEffect, poisonEffect, slowEffect, speedEffect;

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
            speedTicks = 1;
        }

        stunTicks = 1;
        slownessTicks = 1;
        burningTicks = 1;
        weakenedTicks = 1;
        abductionTicks = 1;
        rootedTicks = 1;
        magnetisedTicks = 1;
        poisonTicks = 1;

        StatusUpdate();
    }
    #endregion

    public void StatusUpdate() {
        isBurning();
        isStunned();
        isPoisonned();
        HealthRegeneration();
        isSlow();
        isSpeed();
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
        }

        poisonTicks--;
        health.Damage(poisonDamage);
    }

    [Server]
    public void isSpeed() {
        if (speedTicks == 0)
            return;

        if (speedTicks == 1) {
            RpcEffect("SpeedEmitter", false);
            controller.EffectEnd("Speed");
            speedPerc = 0;
        }

        speedTicks--;
    }

    [Server]
    public void isSlow() {
        if (slownessTicks == 0)
            return;

        if (slownessTicks == 1) {
            RpcEffect("SlowEmitter", false);
            controller.EffectEnd("Slow");
            slownessPerc = 0;
        }

        slownessTicks--;
    }
    #endregion

    #region AddingEffects
    /// <summary>
    /// Generic Status effect function that can apply any status effect, damage is damage per tick on effects that modify health and a modifier for effects that alter stats
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
        if (effectName == StatusEffect.Slow) {
            if (ticks < slownessTicks)
                return;
            controller.EffectStart("Slow", damage);
            RpcEffect("SlowEmitter", true); //Start particle emitter
            slownessTicks = ticks;
            slownessPerc = damage;
        }
        else if (effectName == StatusEffect.Speed) {
            if (ticks < speedTicks)
                return;
            controller.EffectStart("Speed", damage);
            RpcEffect("SpeedEmitter", true);
            speedTicks = ticks;
            speedPerc = damage;
        }
        else if (effectName == StatusEffect.Stun) {
            if (ticks < stunTicks)
                return;
            controller.EffectStart("Stun", damage);

            RpcEffect("StunEmitter", true);
            stunTicks = ticks;
        }
        else if (effectName == StatusEffect.Burn) {
            if (ticks < burningTicks)
                return;

            RpcEffect("BurningEmitter", true); //Start particle emitter
            burningTicks = ticks;
            burningDamage = damage;
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
        if (emitterName.Equals("SlowEmitter"))
            Effect(slowEffect, on);
        else if (emitterName.Equals("SpeedEmitter"))
            Effect(speedEffect, on);
        else if (emitterName.Equals("BurningEmitter"))
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