using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Helpers = Crotty.Helpers.StaticHelpers;

public class BertAttack : PlayerAttack { //Uses Networked Child transform so no need for any Cmd/RPC
    [SerializeField] Transform sawBladeParent, pof;
<<<<<<< Updated upstream
    [SerializeField] float hitsPerSec, bladeSpeed, maxDistance, damageSphereRadius, damage, slowStrength, selfSlowStrength;
=======
    [SerializeField] float hitsPerSec, bladeSpeed, maxDistance, damageSphereRadius, damage, slowStrength, selfSlowStrength, rotSpeed = 120f;
>>>>>>> Stashed changes
    [SerializeField] int slowTicks;
    [SerializeField] LayerMask layers;
    [SerializeField] Vector3 restingOffset;
    float hitTimer, attackTimer;
    Camera cam;
    Inputs inputs;

    override protected void Start() {
        base.Start();
        sawBladeParent.parent = null;
        if (hasAuthority) {
            cam = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Camera>();
            inputs = FindObjectOfType<Inputs>();
            pof.parent = null;
        }
    }

    private void Update() {
        if (!hasAuthority)
            return;

        if (attackTimer > 0) {
            if (Helpers.Vector3Distance(transform.position, sawBladeParent.position) < maxDistance) 
                sawBladeParent.transform.LookAt(pof);
            else 
                sawBladeParent.transform.LookAt(transform.position);

            sawBladeParent.position += sawBladeParent.forward * Time.deltaTime * bladeSpeed;

            attackTimer -= Time.deltaTime;
<<<<<<< Updated upstream
            sawBladeParent.GetChild(0).RotateAround(sawBladeParent.GetChild(0).position, sawBladeParent.transform.up, 360f * Time.deltaTime);
=======
            sawBladeParent.GetChild(0).RotateAround(sawBladeParent.GetChild(0).position, sawBladeParent.transform.up, rotSpeed * Time.deltaTime);
>>>>>>> Stashed changes
            GetComponent<Status>().AddEffect(Status.StatusEffect.Slow, slowTicks, selfSlowStrength);
            hitTimer -= Time.deltaTime;

            if(hitTimer < 0) {
                hitTimer = 1f / hitsPerSec;

#if UNITY_EDITOR
                pos = sawBladeParent.position;
#endif
                Collider[] hits = Physics.OverlapSphere(sawBladeParent.position, damageSphereRadius, layers, QueryTriggerInteraction.Ignore);
                foreach (Collider other in hits) {
                    if (other.tag.Equals("minion") || other.tag.Equals("Player")) {
                        if (other.GetComponent<Team>().GetTeam() != GetComponent<Team>().GetTeam()) {
                            other.GetComponent<Health>().Damage(damage);
                            other.GetComponent<Status>().AddEffect(Status.StatusEffect.Slow, slowTicks, slowStrength);
                        }
                    }
                }
            }
        }
        else if (attackTimer <= 0) {
            pof.transform.position = transform.position + restingOffset;

            if (Helpers.Vector3Distance(pof.position, sawBladeParent.position) > 0.25f) {
                sawBladeParent.transform.LookAt(pof);
                sawBladeParent.position += sawBladeParent.forward * Time.deltaTime * bladeSpeed;
            }
        }
    }

    public override void Attack() {
        Ray cameraRay = cam.ScreenPointToRay(inputs.GetMousePosition());
        if (Physics.Raycast(cameraRay, out RaycastHit hit, 999f, layers, QueryTriggerInteraction.Ignore)) {
            pof.position = hit.point;
            attackTimer = 0.25f;
        }
    }

#if UNITY_EDITOR
    Vector3 pos;
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pos, damageSphereRadius);
    }
#endif
}
