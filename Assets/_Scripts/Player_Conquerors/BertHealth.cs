using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BertHealth : PlayerHealth {
    /// <summary>
    /// Bert required a special health class in order to have his passive domeShield
    /// </summary>
    [SerializeField] float shieldCooldown, shieldTimeActive;
    [SerializeField] float shieldHealth;
    [SerializeField] GameObject shield;
    float shieldTimer, currentShieldHealth;

    public enum ShieldState { Active, Ready, Cooldown}
    ShieldState shieldState = ShieldState.Ready;

    private void FixedUpdate() {
        if (!isServer)
            return;
        if (shieldTimer >= 0) {
            shieldTimer -= Time.deltaTime;
            if(shieldTimer < 0) {
                if(shieldState == ShieldState.Active) {
                    ActiveOver();
                }
                else if (shieldState == ShieldState.Cooldown){
                    shieldState = ShieldState.Ready;
                }
            }
        }
    }

    public override void Damage(float damage) {
        if(shieldState == ShieldState.Ready) {
            //ActivateShield
            ShieldActive(true);
            shieldTimer = shieldTimeActive;
            currentShieldHealth = shieldHealth;
            shieldState = ShieldState.Active;
        }
        if(shieldState == ShieldState.Active) {
            currentShieldHealth -= damage;
            if(currentShieldHealth <= 0) {
                ActiveOver();
            }
        }
        else
            base.Damage(damage);
    }

    private void ActiveOver() {
        shieldTimer = shieldCooldown;
        shieldState = ShieldState.Cooldown;
        ShieldActive(false);
    }

    [ClientRpc]
    public void ShieldActive(bool on) {
        shield.SetActive(on);
    }
}