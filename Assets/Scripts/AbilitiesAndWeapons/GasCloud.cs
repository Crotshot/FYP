using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCloud : MonoBehaviour
{
    [SerializeField] float duration, damagePerSecond, finalScale, expansionTime;
    float expansionTimer;

    List<Transform> trackedTransforms = new List<Transform>();

    private void Start() {
        expansionTimer = expansionTime;
    }

    private void FixedUpdate() {
        foreach (Transform form in trackedTransforms) {
            if (form == null) {
                trackedTransforms.Remove(form);
                continue;
            }

            //Damage tracked targets
        }

        if(expansionTimer >= 0) {
            expansionTimer -= Time.deltaTime;
            transform.localScale = Vector3.one * (finalScale * ((expansionTime - expansionTimer) / expansionTime));
        }
        duration -= Time.deltaTime;

        if(duration <= 0) {
            NetworkServer.Destroy(gameObject);
            Destroy(gameObject);
        }
    }

    //Player, minion
    private void OnTriggerEnter(Collider other) {
        if (other.tag.Equals("minion") || other.tag.Equals("Player") && !trackedTransforms.Contains(other.transform)) {
            if (GetComponent<Team>().GetTeam() != other.GetComponent<Team>().GetTeam()) {
                trackedTransforms.Add(other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag.Equals("minion") || other.tag.Equals("Player") && trackedTransforms.Contains(other.transform)) {
            trackedTransforms.Remove(other.transform);
        }
    }
}
