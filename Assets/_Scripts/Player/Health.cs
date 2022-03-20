using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField]protected float maxHealth;
    [SerializeField][SyncVar(hook = "Altered")]protected float currentHealth;
    public UnityEvent<float, float> HealthChanged;
    protected bool dead;

    private void Start() {
        if (HealthChanged == null)
            HealthChanged = new UnityEvent<float, float>();
    }

    public virtual void Damage(float damage) {
        if (!isServer) {
            CmdDamage(damage);
            return;
        }
        //Debug.Log("GameObject: " + gameObject.name + " took: " + damage + " damage");
        currentHealth -= damage;

        if (currentHealth <= 0) {
            dead = true;
            GetComponent<Status>().DeInit();
        }
    }

    public virtual void Heal(float amount) {
        if (!isServer) {
            CmdHeal(amount);
            return;
        }
        //Debug.Log("GameObject: " + gameObject.name + " took: " + damage + " damage");
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    [Command(requiresAuthority = false)]
    void CmdHeal(float amount) {
        Heal(amount);
    }


    private void Altered(float oldHealth, float newHealth) {
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    [Command (requiresAuthority = false)]
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
        //Debug.Log("GameObject: " + gameObject.name + " health reset to: " + currentHealth);
    }

    [Command]
    private void CmdResetHealth() {
        ResetHealth();
    }

    public float GetCurrentHealth() {
        return currentHealth;
    }

    public float GetMaxHealth() {
        return maxHealth;
    }
}