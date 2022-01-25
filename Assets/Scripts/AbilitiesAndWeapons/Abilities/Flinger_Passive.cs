using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Flinger_Passive : NetworkBehaviour
{
    [SerializeField] float expRad, damage;
    [SerializeField] GameObject particles;
    bool armed = true;

    void Start()
    {
        if (!isServer)
            return;
        GetComponent<Health>().HealthChanged.AddListener(Explode);
    }

    private void Explode(float health, float maxHealth) {
        if (!isServer)
            return;
        if (health == maxHealth) { //Re-arm on respawn
            armed = true;
        }
        if (health <= 0) {
            GameObject obj = Instantiate(particles, transform.position, transform.rotation);
            NetworkServer.Spawn(obj);
            obj.transform.position = transform.position;
            obj.transform.position = transform.position;
            obj.transform.parent = null;

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, expRad, transform.forward * 0.01f);
            if (hits.Length > 0) {
                foreach (RaycastHit hit in hits) { //Friendly fire >:)
                    if (hit.collider.transform.TryGetComponent(out Health hp)) {
                        hp.Damage(damage);
                    }
                }
            }
        }
    }
}
