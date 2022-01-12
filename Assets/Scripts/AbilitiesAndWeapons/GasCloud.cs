using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCloud : NetworkBehaviour
{
    [SerializeField] float duration, damagePerSecond, finalScale, expansionTime, minTime;
    float expansionTimer;

    List<Health> trackedHealth = new List<Health>();

    private void Start() {
        if (!isServer)
            Destroy(this);
        expansionTimer = expansionTime;
    }

    private void FixedUpdate() {
        foreach (Health hp in trackedHealth) {
            if (hp == null) {
                trackedHealth.Remove(hp);
                continue;
            }
            hp.Damage(damagePerSecond * Time.deltaTime); //Maybe move to only call every .1 seconds
        }

        if(expansionTimer >= 0) {
            expansionTimer -= Time.deltaTime;
            transform.localScale = Vector3.one * (finalScale * ((expansionTime - expansionTimer + minTime) / (expansionTime + minTime)));
        }
        duration -= Time.deltaTime;

        if(duration <= 0) {
            NetworkServer.Destroy(gameObject);
            Destroy(gameObject);
        }
    }

    //Player, minion
    private void OnTriggerEnter(Collider other) {
        if (other.tag.Equals("minion") || other.tag.Equals("Player")) {
            if (other.TryGetComponent(out Health health)) {
                if (health.GetComponent<Team>().GetTeam() != GetComponent<Team>().GetTeam()) {
                    trackedHealth.Add(health);
                }
                
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag.Equals("minion") || other.tag.Equals("Player")) {
            if (TryGetComponent(out Health health)) {
                if (trackedHealth.Contains(health)) {
                    trackedHealth.Remove(health);
                }

            }
        }
    }
}
