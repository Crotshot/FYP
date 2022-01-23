using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers = Crotty.Helpers.StaticHelpers;
using Mirror;

public class ArchingProjectile : NetworkBehaviour
{
    
    [SerializeField] float damage, archHeight, range, airTime;
    Vector3 targPos, startPos;
    float timer, t;
    [SerializeField] Vector3[] directions;
    [SerializeField] float rayLength;

    public void Setup(Vector3 targetedPosition) {
        if (!isServer)
            Destroy(this);
        targPos = targetedPosition;
        startPos = transform.position;
        float distance = Helpers.Vector3Distance(transform.position, targPos);
        if(distance > range) {
            airTime *= distance / range;
            archHeight*= distance / range;
        }
    }

    private void FixedUpdate() {
        if(targPos != null) {
            timer += Time.deltaTime / airTime;
            t = timer / airTime;

            transform.position = new Vector3(startPos.x * (1 - t) + targPos.x * t,  //x
                                  (1 - t) * (t) * (archHeight) + (startPos.y + targPos.y) / 2f, //y
                                  startPos.z * (1 - t) + targPos.z * t); //z

            transform.LookAt(new Vector3(startPos.x * (1 - t * 1.1f) + targPos.x * t * 1.1f,  //x
                                  (1 - t * 1.1f) * (t * 1.1f) * (archHeight) + (startPos.y + targPos.y) / 2f, //y
                                  startPos.z * (1 - t * 1.1f) + targPos.z * t * 1.1f)); //z
            if (t >= 1.2f)
                NetworkServer.Destroy(gameObject);

            foreach (Vector3 direction in directions) {
                Debug.DrawRay(transform.position, transform.TransformDirection(direction) * rayLength, Color.yellow/*, 0.1f*/);
                Ray ray = new Ray(transform.position, transform.TransformDirection(direction));
                if (Physics.Raycast(ray, out RaycastHit hit, rayLength) && hit.collider.isTrigger == false) {
                    if (hit.collider.transform.TryGetComponent(out Team t)) {
                        if (t.GetTeam() == GetComponent<Team>().GetTeam())
                            return;
                    }
                    if (hit.collider.transform.TryGetComponent(out Health health)) {
                        health.Damage(damage);
                        NetworkServer.Destroy(gameObject);
                    }
                    break;
                }
            }

        }
    }
}
