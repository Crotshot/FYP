using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField] float maxHealth;//, regenDelay;
    //[Range(0, 1)]
    //[SerializeField] float regenPercentPerSec;
    //float regenTimer;
    [SyncVar][SerializeField] private float currentHealth;

    public event Action<float> Damaged;
    public event Action Dead;

    public void Damage(float damage) {
        if (isServer) {
            currentHealth -= damage;
            //regenTimer = regenDelay;
            if (currentHealth != 0) {
                Damaged?.Invoke(currentHealth);
                return;
            }
            Dead?.Invoke();
        }
        else {
            CmdDamage(damage);
        }
    }

    [Command]
    void CmdDamage(float damage) {
        Damage(damage);
    }

    public void ResetHealth() {
        currentHealth = maxHealth;
    }
}

//private void Update() {
//    if (regenTimer > 0) {
//        regenTimer -= Time.deltaTime;
//    }
//    else {
//        RegenerateHealth();
//    }
//}

//[Server]
//public void RegenerateHealth() {
//    if (currentHealth < maxHealth) {
//        currentHealth += regenPercentPerSec * Time.deltaTime;
//        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
//    }
//}