using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth, regenDelay;
    [SerializeField] bool isPlayer;
    [Range(0, 1)]
    [SerializeField] float regenPercentPerSec;
    float currentHealth, regenTimer;
    public int team;

    public UnityEvent<float> damaged;

    private void Awake() {
        if (damaged == null)
            damaged = new UnityEvent<float>();
        team = GetComponent<Team>().GetTeam();
    }

    private void Update() {
        if(regenTimer > 0) {
            regenTimer -= Time.deltaTime;
        }
        else {
            if(currentHealth < maxHealth) {
                currentHealth += regenPercentPerSec * Time.deltaTime;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            }
        }
    }

    public void Damage(float damage) {
        currentHealth -= damage;
        regenTimer = regenDelay;
        damaged?.Invoke(currentHealth);
    }

    public void ResetHealth() {
        currentHealth = maxHealth;
    }

    public int GetTeam() {
        return team;
    }
}