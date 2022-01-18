using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers = Crotty.Helpers.StaticHelpers;

public class MortarProj : NetworkBehaviour
{
    private Vector3 targetPos;
    [SerializeField] GameObject spawnOnCollision;
    [SerializeField] float trackDelay;
    float trackTimer;
    Rigidbody rb;

    private void Start() {
        if (!isServer)
            Destroy(this);
    }

    private void FixedUpdate() {
        if (targetPos == null)
            return;

        if(trackTimer <= 0) {
            if (rb != null)
                rb.AddForce(transform.forward * 100f);
        }
        else {
            trackTimer -= Time.deltaTime;
            if (trackTimer <= 0) {
                rb.velocity = Vector3.zero;
                rb.useGravity = false;
                GetComponent<SphereCollider>().enabled = true;
                transform.position = targetPos + Vector3.up * 50f;
                transform.LookAt(targetPos);
            }
        }
    }

    public void SetTargetPos(Vector3 pos) {
        targetPos = pos;
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 3000f);
        trackTimer = trackDelay;
    }


    private void OnCollisionEnter(Collision collision) {
        GameObject newCloud = Instantiate(spawnOnCollision, transform.position, Quaternion.identity);
        NetworkServer.Spawn(newCloud);//Spawn mortar with mouse pos as target
        newCloud.transform.position = transform.position;
        newCloud.GetComponent<Team>().SetTeam(GetComponent<Team>().GetTeam());
        newCloud.transform.parent = null;
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }
}
