using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Helpers = Crotty.Helpers.StaticHelpers;

public class WizardTower : NetworkBehaviour{

    [SerializeField] Animator animator;
    [SerializeField] Transform towerTop, rotationControl, nozzle;
    [SerializeField] float fireRadius = 10f, tickSpeed = 0.1f;
    [SerializeField] int damagePerTick = 1;

    float animTime, animSpeed, tickTimer;
    bool play, reverse;
    BuildingVision bV;
    StructureCaptureState sCS;

    Vector3 currentPos; // Raycast out of Nozzle and hit point is where a sphere cast tries to hit targets
    ParticleSystem pS;
    int layerD, layerU;

    Vector3 gizmoPos;

    private void Start() {
        pS = nozzle.GetComponent<ParticleSystem>();
        pS.Stop();
        if (!isServer)
            return;
        bV = GetComponent<BuildingVision>();
        sCS = GetComponent<StructureCaptureState>();

        layerD = 1 << LayerMask.NameToLayer("Default");
        layerU = 1 << LayerMask.NameToLayer("Unit");
    }

    private void FixedUpdate() {
        if (play) {
            play = false;
            animator.SetFloat("Speed", 1);
            animSpeed = 1;
        }

        if (reverse) {
            reverse = false;
            animator.SetFloat("Speed", -1);
            animSpeed = -1;
        }

        if (animTime >= 0.63f && animSpeed > 0) {
            animator.SetFloat("Speed", 0);
            animSpeed = 0;
            if(!pS.isEmitting)
                pS.Play();
        }
        else if (animTime <= 0.02f && animSpeed < 0) {
            animator.SetFloat("Speed", 0);
            animSpeed = 0;
        }
        else {
            animTime += animSpeed * Time.deltaTime;

            if (animTime < 0.58f && pS.isEmitting) {
                pS.Stop();
            }

            if (animTime > 0.63f) {
                animTime = 0.63f;
            }
            else if (animTime < 0) {
                animTime = 0;
            }
            
        }

        if (!isServer)
            return;


        Vector3 targetPos = bV.GetTargetPos();

        if (targetPos.y != -999) {
            rotationControl.LookAt(targetPos);
            rotationControl.eulerAngles = new Vector3(0, rotationControl.eulerAngles.y, 0);

            nozzle.LookAt(targetPos);
            nozzle.localEulerAngles = new Vector3(nozzle.eulerAngles.x, 0 , 0);

            if (tickTimer > 0) {
                tickTimer -= Time.deltaTime;
                return;
            }

            //play = true;
            Play();
            Ray ray = new Ray(nozzle.position, nozzle.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 100, layerD, QueryTriggerInteraction.Ignore)) {
                gizmoPos = hit.point;
                RaycastHit[] hits = Physics.SphereCastAll(hit.point, fireRadius, transform.forward * 0.01f, 0, layerU, QueryTriggerInteraction.Ignore);
                if (hits.Length > 0) {
                    foreach (RaycastHit rhit in hits) {
                        if (rhit.transform.TryGetComponent(out Team team) && rhit.transform.TryGetComponent(out Health h)) {
                            if (team.GetTeam() != sCS.getOwningTeam()) {
                                h.Damage(damagePerTick);
                                tickTimer = tickSpeed;
                            }
                        }
                    }
                }
            }

        }
        else {
            //reverse = true;
            Reverse();
        }

        towerTop.rotation = Quaternion.Lerp(towerTop.rotation, rotationControl.rotation, 10f * Time.deltaTime);
        RpcUpdateRotation(towerTop.rotation.x, towerTop.rotation.y, towerTop.rotation.z, towerTop.rotation.w, nozzle.rotation.x, nozzle.rotation.y, nozzle.rotation.z, nozzle.rotation.w);
    }

    [ClientRpc]
    private void RpcUpdateRotation(float x1, float y1, float z1, float w1, float x2, float y2, float z2, float w2) {
        towerTop.rotation = new Quaternion(x1,y1,z1,w1);
        nozzle.rotation = new Quaternion(x2, y2, z2, w2);
    }

    [ClientRpc]
    private void Play() {
        play = true;
    }

    [ClientRpc]
    private void Reverse() {
        reverse = true;
    }


    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(gizmoPos, fireRadius);
    }
}
