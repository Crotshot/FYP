using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FixedBallista : NetworkBehaviour
{
    [SerializeField] Transform ballista, rotationControl, bolt, boltRest;
    [SerializeField] float boltYHeight, airTime, coolDown, waitTime, aoeDamage, aoeRadius,inaccuracy = 3f;
    [SerializeField] ParticleSystem pS;
    Vector3 ballistaAngle;
    StructureCaptureState sCS;
    int layerD, layerU;

    public enum BallistaState { AirTimer, WaitTime, Cooldown }
    public BallistaState ballistaState = BallistaState.Cooldown;

    float airTimeTimer, shotCooldownTimer, waitTimer;
    Vector3 boltStart, boltEnd;
    List<Transform> trackedTransforms;
    Transform target;

    private void Start() {
        trackedTransforms = new List<Transform>();
        if (!isServer) {
            return;
        }
        sCS = GetComponent<StructureCaptureState>();

        layerD = 1 << LayerMask.NameToLayer("Default");
        layerU = 1 << LayerMask.NameToLayer("Unit");
    }

    private void FixedUpdate() {
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

        if (ballistaState == BallistaState.Cooldown) {
            bolt.position = boltRest.position;
            bolt.rotation = boltRest.rotation;
            if (isServer) {
                shotCooldownTimer -= Time.deltaTime;
                if (shotCooldownTimer < 0 && target != null) {
                    RpcShoot(bolt.position, target.position + target.forward + new Vector3(Random.Range(-inaccuracy, inaccuracy),0, Random.Range(-inaccuracy, inaccuracy)));
                }
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
                pS.Stop();
            }
            else {
                waitTimer -= Time.deltaTime;
            }
        }
    }

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
    }
}
