using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Helpers = Crotty.Helpers.StaticHelpers;

public class BuildingVision : NetworkBehaviour
{
    [SerializeField] Vector3 spherePos;
    [SerializeField] float sensorRadius, interval;
    float intervalTimer;
    int layer;
    StructureCaptureState sCS;
    Transform target;

    private void Start() {
        if (!isServer) {
            return;
        }

        layer = 1 << LayerMask.NameToLayer("Unit");
        sCS = GetComponent<StructureCaptureState>();
        intervalTimer = interval;
    }

    private void FixedUpdate() {
        if(interval >= 0) {
            interval -= Time.deltaTime;
            return;
        }
        if(target == null || Helpers.Vector3Distance(target.position, transform.position + spherePos) > sensorRadius ){//&& sCS.getOwningTeam() != 0) {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position + spherePos, sensorRadius, transform.forward * 0.01f, 0, layer, QueryTriggerInteraction.Ignore);
            if (hits.Length > 0) {
                foreach (RaycastHit hit in hits) {
                    if (hit.transform.TryGetComponent(out Team team)) {
                        if (team.GetTeam() != sCS.getOwningTeam()) {
                            target = hit.transform;
                            interval = intervalTimer;
                        }
                    }
                }
            }
        }
        else {
            target = null;
            interval = intervalTimer;
        }
    }


    public Vector3 GetTargetPos() {
        if (target == null)
            return Vector3.one * -999f;
        return target.position;
    }

    //private void OnDrawGizmos() {
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawSphere(transform.position + spherePos, sensorRadius);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawCube(GetTargetPos(), Vector3.one * 3f);
    //}
}
