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
            Debug.Log("Uhh ohh Stinkee");
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
        Debug.Log("Stinky Cloud touched");
        if (other.tag.Equals("minion") || other.tag.Equals("Player")) {
            Debug.Log("Minion or player");
            if (other.TryGetComponent(out Health health)) {
                Debug.Log("Health acquired");
                if (health.GetComponent<Team>().GetTeam() != GetComponent<Team>().GetTeam()) {
                    Debug.Log("Time to stink");
                    trackedHealth.Add(health);
                }
                
            }
            else {
                Debug.Log("Did not grab health script");
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
