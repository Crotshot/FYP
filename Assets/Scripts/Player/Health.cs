using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField]protected float maxHealth;
    [SerializeField]protected float currentHealth;
    public event Action<float> Damaged;
    bool dead;

    public void Damage(float damage) {
        if (isServer) {
            RpcDamage(damage);
        }
        else {
            currentHealth -= damage;
            if(currentHealth <= 0) {
                dead = true;
            }
        }
    }

    [ClientRpc]
    void RpcDamage(float damage) {
        Damage(damage);
    }

    public void ResetHealth() {
        if (isServer) {
            RpcResetHealth();
        }
        else {
            currentHealth = maxHealth;
            dead = false;
        }
    }

    [ClientRpc]
    private void RpcResetHealth() {
        ResetHealth();
    }
}