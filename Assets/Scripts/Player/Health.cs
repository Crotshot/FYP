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
    [SyncVar(hook = nameof(HealthChanged))][SerializeField] private float currentHealth;
    
    public event Action<float> Damaged;
    public event Action Dead;

    List<GameObject> spawnPoints = new List<GameObject>(); //Temp respawn code


    //TEST
    public bool die;
    //

    private void Start() {
        Setup();
    }

    private void Setup() {
        if (isServer) {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Respawn"))
                spawnPoints.Add(obj);
        }
        else {
            CmdSetup();
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Respawn"))
                spawnPoints.Add(obj);
        }
    }

    [Command]
    void CmdSetup() {
        Setup();
    }


    void HealthChanged(float oldHealth, float newHealth) {
        if (newHealth >= oldHealth)
            return;
        if (currentHealth > 0) {
            Damaged?.Invoke(currentHealth);
            return;
        }
        Respawn();
    }

    public void Damage(float damage) {
        if (isServer) {
            currentHealth -= damage;
        }
        else {
            CmdDamage(damage);
        }
    }

    [Command]
    void CmdDamage(float damage) {
        Damage(damage);
    }

    void Respawn() {
        if (isServer) {
            RPCRespawn();
        }
        else {
            Dead?.Invoke();
            ResetHealth();
            transform.position = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)].transform.position;
        }
    }

    [ClientRpc]
    void RPCRespawn() {
        Dead?.Invoke();
        ResetHealth();
        transform.position = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)].transform.position;
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