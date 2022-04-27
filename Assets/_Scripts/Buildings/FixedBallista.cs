using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FixedBallista : NetworkBehaviour
{
    [SerializeField] Transform ballista, rotationControl, bolt, boltRest;
<<<<<<< Updated upstream
    [SerializeField] float boltYHeight, airTime, coolDown, waitTime, aoeDamage, aoeRadius;
    [SerializeField] ParticleSystem pS;
    Vector3 ballistaAngle;
    BuildingVision bV;
=======
    [SerializeField] float boltYHeight, airTime, coolDown, waitTime, aoeDamage, aoeRadius,inaccuracy = 3f;
    [SerializeField] ParticleSystem pS;
    Vector3 ballistaAngle;
>>>>>>> Stashed changes
    StructureCaptureState sCS;
    int layerD, layerU;

    public enum BallistaState { AirTimer, WaitTime, Cooldown }
    public BallistaState ballistaState = BallistaState.Cooldown;

    float airTimeTimer, shotCooldownTimer, waitTimer;
    Vector3 boltStart, boltEnd;
<<<<<<< Updated upstream


    private void Start() {
        if (!isServer)
            return;
        bV = GetComponent<BuildingVision>();
=======
    List<Transform> trackedTransforms;
    Transform target;

    private void Start() {
        trackedTransforms = new List<Transform>();
        if (!isServer) {
            return;
        }
>>>>>>> Stashed changes
        sCS = GetComponent<StructureCaptureState>();

        layerD = 1 << LayerMask.NameToLayer("Default");
        layerU = 1 << LayerMask.NameToLayer("Unit");
    }

    private void FixedUpdate() {
<<<<<<< Updated upstream
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
=======
        if((target == null || trackedTransforms.Contains(target) || target.position.y < -10) && trackedTransforms.Count > 0 && sCS.getOwningTeam() != 0) {
            for(int i = trackedTransforms.Count-1; i > -1; i--) {
                if(trackedTransforms[i] == null || trackedTransforms[i].GetComponent<Team>().GetTeam() == sCS.getOwningTeam()) {
                    trackedTransforms.Remove(trackedTransforms[i]);
                    target = null;
                }
                else {
                    target = trackedTransforms[i];
                    break;
                }
            }
        }
        if (target != null) rotationControl.LookAt(target.position); else rotationControl.localEulerAngles = Vector3.zero;
        ballista.rotation = Quaternion.Slerp(ballista.rotation, rotationControl.rotation, 2f * Time.deltaTime);
>>>>>>> Stashed changes

        if (ballistaState == BallistaState.Cooldown) {
            bolt.position = boltRest.position;
            bolt.rotation = boltRest.rotation;
<<<<<<< Updated upstream
            RpcBolt(bolt.position, bolt.localEulerAngles);
            shotCooldownTimer -= Time.deltaTime;
            if (shotCooldownTimer < 0 && targetPos.y != -999) {
                ballistaState = BallistaState.AirTimer;
                airTimeTimer = 0;
                boltStart = bolt.position;
                boltEnd = targetPos;
=======
            if (isServer) {
                shotCooldownTimer -= Time.deltaTime;
                if (shotCooldownTimer < 0 && target != null) {
                    RpcShoot(bolt.position, target.position + target.forward + new Vector3(Random.Range(-inaccuracy, inaccuracy),0, Random.Range(-inaccuracy, inaccuracy)));
                }
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
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
=======
            if (airTimeTimer >= 1f) {
                ballistaState = BallistaState.WaitTime;
                waitTimer = waitTime;

                pS.Play();                //AOE damage
                if (isServer) {
                    RaycastHit[] hits = Physics.SphereCastAll(bolt.position, aoeRadius, transform.forward * 0.01f, 0, layerU, QueryTriggerInteraction.Ignore);
                    if (hits.Length > 0) {
                        foreach (RaycastHit rhit in hits) {
                            if (rhit.transform.TryGetComponent(out Team team) && rhit.transform.TryGetComponent(out Health h)) {
                                if (team.GetTeam() != sCS.getOwningTeam()) {
                                    h.Damage(aoeDamage);
                                }
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
                RpcEffect(false);
=======
                pS.Stop();
>>>>>>> Stashed changes
            }
            else {
                waitTimer -= Time.deltaTime;
            }
        }
    }

<<<<<<< Updated upstream
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
=======
    private void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent(out Health hp)) {
            if(hp.GetComponent<Team>().GetTeam() != sCS.getOwningTeam() && !trackedTransforms.Contains(other.transform)) {
                trackedTransforms.Add(other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out Health hp)) {
            if (trackedTransforms.Contains(other.transform)) {
                trackedTransforms.Remove(other.transform);
            }
        }
    }

    [ClientRpc]
    private void RpcShoot(Vector3 start, Vector3 end) {
        ballistaState = BallistaState.AirTimer;
        airTimeTimer = 0;
        boltStart = start;
        boltEnd = end;
>>>>>>> Stashed changes
    }
}
