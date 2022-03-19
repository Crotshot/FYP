using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCloud : NetworkBehaviour
{
    [SerializeField] float duration, gasDamage, scalePerSec;
    [SerializeField] int poisonTicks;
    float expansionTimer, tickInterval = 0.25f, ticktimer = 0.25f;

    List<Status> trackedStatus = new List<Status>();

    private void Start() {
        if (!isServer)
            Destroy(this);
    }

    private void FixedUpdate() {
        if (ticktimer > 0) {
            ticktimer -= Time.deltaTime;
        }
        else {
            for(int i = trackedStatus.Count - 1; i > -1; i--) {
                if (trackedStatus[i] == null) {
                    trackedStatus.Remove(trackedStatus[i]);
                    continue;
                }
                trackedStatus[i].AddEffect(Status.StatusEffect.Poison, poisonTicks, gasDamage);
            }
            ticktimer = tickInterval;
        }

        if(expansionTimer >= 0) {
            expansionTimer -= Time.deltaTime;
            transform.localScale += Vector3.one * scalePerSec * Time.deltaTime;
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
            if (other.GetComponent<Team>().GetTeam() != GetComponent<Team>().GetTeam()) {
                trackedStatus.Add(other.GetComponent<Status>());
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag.Equals("minion") || other.tag.Equals("Player")) {
            if (other.TryGetComponent(out Status s)) {
                if (trackedStatus.Contains(s)) {
                    trackedStatus.Remove(s);
                }
            }
        }
    }
}
