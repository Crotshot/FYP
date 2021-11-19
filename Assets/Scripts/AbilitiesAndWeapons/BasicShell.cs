using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BasicShell :  NetworkBehaviour
{
    [SerializeField] float shellSpeed = 2f, deleteTime = 10f, damage = 20f, rayLength = 0.5f;
    [SerializeField] Vector3 rayOrigin;
    [SerializeField] Vector3[] directions;

    private void Start() {
        Invoke(nameof(DestroySelf), deleteTime);
    }

    void Update() {
        transform.Translate(Vector3.forward * Time.deltaTime * shellSpeed, Space.Self);
        foreach (Vector3 direction in directions) {
            Debug.DrawRay(transform.position + transform.TransformDirection(rayOrigin), transform.TransformDirection(direction) * rayLength, Color.blue/*, 0.1f*/);
            Ray ray = new Ray(transform.position + transform.TransformDirection(rayOrigin), transform.TransformDirection(direction));
            if (Physics.Raycast(ray, out RaycastHit hit, rayLength)) {
                RayCollision(hit.collider.transform);
            }
        }
    }

    private void RayCollision(Transform hit) {
        if (isServer) {
            if (hit.TryGetComponent(out NetworkIdentity networkIdentity)) {
                if (networkIdentity.connectionToClient == connectionToClient)
                    return;
            }
            if (hit.TryGetComponent(out Health health)) {
                health.Damage(damage);
            }
            DestroySelf();
        }
        else {
            CmdRayCollision(hit);
        }
    }

    [Command]
    void CmdRayCollision(Transform h) {
        RayCollision(h);
    }

    private void DestroySelf() {
        NetworkServer.Destroy(gameObject);
    }
}