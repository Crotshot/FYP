using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField]protected float maxHealth;
    [SerializeField][SyncVar]protected float currentHealth;
    public event Action<float> Damaged;
    protected bool dead;

    public virtual void Damage(float damage) {
        if (!isServer) {
            CmdDamage(damage);
            return;
        }
        Debug.Log("GameObject: " + gameObject.name + " took: " + damage + " damage");
        currentHealth -= damage;
        if(currentHealth <= 0) {
            dead = true;
        }
    }

    [Command]
    void CmdDamage(float damage) {
        Damage(damage);
    }

    public void ResetHealth() {
        if (!isServer) {
            CmdResetHealth();
            return;
        }
        currentHealth = maxHealth;
        dead = false;
        Debug.Log("GameObject: " + gameObject.name + " health reset to: " + currentHealth);
    }

    [Command]
    private void CmdResetHealth() {
        ResetHealth();
    }
}