using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Mirror;

public class MinionHealth : Health { //Class for tracking minion health
                                     //On Death => Disable navAgent, disable controller, move to 0,-60, 0, setTeam 0, Add to pool //POOL CURRENTLY DISABLED
                                     //On Respawn from pool => remove from pool, set pos, enable navAgent, enable controller, setTeam, reset Health;
    [SerializeField] GameObject deathPrefab;

    public override void Damage(float damage) {
        if (!dead) {
            base.Damage(damage);
            if (dead) {
                if (!isServer)
                    return;

                RpcDeathParticles();
                GetComponent<NavMeshAgent>().enabled = false;
                GetComponent<MinionController>().MinionDeath();
                GetComponent<MinionController>().enabled = false;
                transform.position = new Vector3(0, -60, 0);
                GetComponent<Team>().SetTeam(0);
                FindObjectOfType<MinionManager>().RemoveMinion(GetComponent<MinionController>());
                FindObjectOfType<MinionPool>().AddMinionToPool(gameObject);
            }
        }
    }

    [ClientRpc]
    private void RpcDeathParticles() {
        Instantiate(deathPrefab, transform.position, transform.rotation);
    }
}