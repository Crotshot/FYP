using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Flinger_Passive : NetworkBehaviour
{
    [SerializeField] float expRad, damage;
    [SerializeField] GameObject particles;
    bool armed = true;
    int layer;

    void Start() {
        if (!isServer)
            return;
        GetComponent<Health>().HealthChanged.AddListener(Explode);
        layer = 1 << LayerMask.NameToLayer("Unit");
    }

    private void Explode(float health, float maxHealth) {
        if (!isServer)
            return;
        if (health == maxHealth) { //Re-arm on respawn
            armed = true;
        }
        if (health <= 0) {
            RpcDeathBlast();
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, expRad, transform.forward * 0.01f, 0, layer, QueryTriggerInteraction.Ignore);
            if (hits.Length > 0) {
                foreach (RaycastHit hit in hits) { //Friendly fire >:)
                    if (hit.collider.transform.TryGetComponent(out Health hp)) {
                        hp.Damage(damage);
                    }
                }
            }
        }
    }

    [Client]
    private void RpcDeathBlast() {
        GameObject obj = Instantiate(particles, transform.position, transform.rotation);
        obj.transform.parent = null;
    }
}
