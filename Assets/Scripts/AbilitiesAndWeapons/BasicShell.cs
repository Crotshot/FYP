using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicShell : MonoBehaviour
{
    [SerializeField] float shellSpeed = 20f, deleteTime = 10f, damage = 20f, rayLength = 0.5f;
    [SerializeField] Vector3 rayOrigin;
    [SerializeField] Vector3[] directions;
    int team;
    bool armed = false;

    public void Make(int team) {
        this.team = team;
        armed = true;
    }

    void Update() {
        if (!armed)
            return;
        transform.Translate(Vector3.forward * Time.deltaTime * shellSpeed, Space.Self);
        deleteTime -= Time.deltaTime;
        if (deleteTime <= 0) {
            Destroy(gameObject);
        }
        foreach (Vector3 direction in directions) {
            Debug.DrawRay(transform.position + transform.TransformDirection(rayOrigin), transform.TransformDirection(direction) * rayLength, Color.blue/*, 0.1f*/);
            Ray ray = new Ray(transform.position + transform.TransformDirection(rayOrigin), transform.TransformDirection(direction));
            if (Physics.Raycast(ray, out RaycastHit hit, rayLength)) {
                if (hit.collider.TryGetComponent(out Health hp)) {
                    if (hp.GetTeam() != team) {
                        hp.Damage(damage);
                        Destroy(gameObject);
                    }
                }
                else {
                    Destroy(gameObject);
                }
            }
        }
    }
}