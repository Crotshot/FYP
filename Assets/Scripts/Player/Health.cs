using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth, regenDelay;
    int team = 0;
    [SerializeField] bool isPlayer;
    [Range(0, 1)]
    [SerializeField] float regenPercentPerSec;
    float currentHealth, regenTimer;

    public UnityEvent<float> damaged;

    private void Awake() {
        if (damaged == null)
            damaged = new UnityEvent<float>();
        if (isPlayer) {
//RE-ENABLE            team = GetComponent<PlayerController>().GetTeam();
        }
        else {
            team = Random.Range(100,100000);//Will need to be changed later for minions
        }
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

    public int GetTeam() {
        return team;
    }

    public void ResetHealth() {
        currentHealth = maxHealth;
    }
}
