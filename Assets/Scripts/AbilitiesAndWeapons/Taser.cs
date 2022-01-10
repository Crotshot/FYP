using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StaticHelpers = Crotty.Helpers.StaticHelpers;

public class Taser : Ability {
    [SerializeField] Transform spawnPoint;
    ParticleSystem pS;
    [SerializeField] float range, damage, stunTime;
    private void Start() {
        SetUp(Cast);
        pS = spawnPoint.GetComponent<ParticleSystem>();
    }

    private void FixedUpdate() {
        CoolDown(Time.deltaTime);
    }

    private void Cast() {
        if (AbilityUsed()) {
            //Raycast out of weapon & Get collision obsticle and point
            Debug.DrawRay(spawnPoint.position, spawnPoint.forward * range , Color.blue, 2.5f);
            Ray ray = new Ray(spawnPoint.position, spawnPoint.forward);
            float halfDist = range / 2.0f;
            if (Physics.Raycast(ray, out RaycastHit hit, range) && hit.collider.isTrigger == false) {
                Debug.Log("Taser hit Object: " + hit.collider.name + " at position" + hit.point);
                halfDist = StaticHelpers.Vector3Distance(hit.point, spawnPoint.position) / 2.0f;

                //DEAL DAMAGE & STUN



                //
            }
            else
                Debug.Log("Taser miss");
            
            var shape = pS.shape;
            shape.radius = halfDist;                //Set Radius to 1/2 dist of hit.point and spawnPoint
            shape.position = new Vector3(0, 0, halfDist);
            //Play burst
            pS.Play();
        }
    }
}