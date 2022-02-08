using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FixedBallista : NetworkBehaviour
{
    [SerializeField] Transform ballista, rotationControl, bolt, boltRest;
    [SerializeField] float boltYHeight, airTime, coolDown, waitTime, aoeDamage, aoeRadius;
    [SerializeField] ParticleSystem pS;
    Vector3 ballistaAngle;
    BuildingVision bV;
    StructureCaptureState sCS;
    int layerD, layerU;

    public enum BallistaState { AirTimer, WaitTime, Cooldown }
    public BallistaState ballistaState = BallistaState.Cooldown;

    float airTimeTimer, shotCooldownTimer, waitTimer;
    Vector3 boltStart, boltEnd;


    private void Start() {
        if (!isServer)
            return;
        bV = GetComponent<BuildingVision>();
        sCS = GetComponent<StructureCaptureState>();

        layerD = 1 << LayerMask.NameToLayer("Default");
        layerU = 1 << LayerMask.NameToLayer("Unit");
    }

    private void FixedUpdate() {
        if (!isServer)
            return;

        Vector3 targetPos = bV.GetTargetPos();
        if (targetPos.y != -999) {
            rotationControl.LookAt(targetPos);
            rotationControl.eulerAngles = new Vector3(0, Mathf.Clamp(rotationControl.eulerAngles.y, 45.2f, -45.2f), 0);

            Debug.Log(rotationControl.localEulerAngles.y);
            if (rotationControl.localEulerAngles.y > 315f && rotationControl.localEulerAngles.y > 45f) {
                targetPos.y = -999;
            }
        }
        ballista.rotation = Quaternion.Slerp(ballista.rotation, rotationControl.rotation, 2f * Time.deltaTime);
        RpcUpdateRotation(ballista.rotation.x, ballista.rotation.y, ballista.rotation.z, ballista.rotation.w);

        if (ballistaState == BallistaState.Cooldown) {
            bolt.position = boltRest.position;
            bolt.rotation = boltRest.rotation;
            RpcBolt(bolt.position, bolt.localEulerAngles);
            shotCooldownTimer -= Time.deltaTime;
            if (shotCooldownTimer < 0 && targetPos.y != -999) {
                ballistaState = BallistaState.AirTimer;
                airTimeTimer = 0;
                boltStart = bolt.position;
                boltEnd = targetPos;
            }
        }
        else if (ballistaState == BallistaState.AirTimer) {
            airTimeTimer += Time.deltaTime * (1 / airTime);

            bolt.position = new Vector3(boltStart.x * (1 - airTimeTimer) + boltEnd.x * airTimeTimer,  //x
                                (1 - airTimeTimer) * (airTimeTimer) * (boltYHeight) + boltStart.y * (1 - airTimeTimer) + boltEnd.y * airTimeTimer, //y
                                boltStart.z * (1 - airTimeTimer) + boltEnd.z * airTimeTimer); //z

            bolt.LookAt(new Vector3(boltStart.x * (1 - airTimeTimer * 1.02f) + boltEnd.x * airTimeTimer * 1.02f,  //x
                                    (1 - airTimeTimer * 1.02f) * (airTimeTimer * 1.02f) * (boltYHeight) + boltStart.y * (1 - airTimeTimer) + boltEnd.y * airTimeTimer, //y
                                    boltStart.z * (1 - airTimeTimer * 1.02f) + boltEnd.z * airTimeTimer * 1.02f)); //z
            RpcBolt(bolt.position, bolt.localEulerAngles);
            if (airTimeTimer >= 1f) {
                ballistaState = BallistaState.WaitTime;
                waitTimer = waitTime;
                //AOE damage
                RpcEffect(true);
                RaycastHit[] hits = Physics.SphereCastAll(bolt.position, aoeRadius, transform.forward * 0.01f, 0, layerU, QueryTriggerInteraction.Ignore);
                if (hits.Length > 0) {
                    foreach (RaycastHit rhit in hits) {
                        if (rhit.transform.TryGetComponent(out Team team) && rhit.transform.TryGetComponent(out Health h)) {
                            if (team.GetTeam() != sCS.getOwningTeam()) {
                                h.Damage(aoeDamage);
                            }
                        }
                    }
                }
            }
        }
        else { //BallistaState.WaitTime
            if (waitTimer <= 0) {
                ballistaState = BallistaState.Cooldown;
                shotCooldownTimer = coolDown;
                RpcEffect(false);
            }
            else {
                waitTimer -= Time.deltaTime;
            }
        }
    }

    [ClientRpc]
    private void RpcUpdateRotation(float x1, float y1, float z1, float w1) {
        ballista.rotation = new Quaternion(x1, y1, z1, w1);
    }

    [ClientRpc]
    private void RpcBolt(Vector3 pos, Vector3 rot) {
        bolt.position = pos;
        bolt.localEulerAngles = rot;
    }

    [ClientRpc]
    private void RpcEffect(bool play) {
        if (play) {
            pS.Play();
        }
        else {
            pS.Stop();
        }
    }
}
