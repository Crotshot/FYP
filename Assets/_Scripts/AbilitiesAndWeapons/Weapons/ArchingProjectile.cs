using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers = Crotty.Helpers.StaticHelpers;
using Mirror;
/// <summary>
/// In arching projectiles the inverse of projSpeed is used to calculate percent distance travelled form start to end
/// </summary>
public class ArchingProjectile : Projectile
{
    [SerializeField] float archHeight;

    override protected void Update() {
        ProjectileMovement();
        CollisionCheck();
    }

    virtual protected void ProjectileMovement() {
        foreach (Proj p in projectiles) {
            if (p.state == Proj.ProjState.Inactive)
                continue;

            p.maxTime += (1 / projSpeed) * Time.deltaTime;

            p.projBody.position = new Vector3(p.start.x * (1 - p.maxTime) + p.end.x * p.maxTime,  //x
                                  (1 - p.maxTime) * (p.maxTime) * (archHeight) + (p.start.y + p.end.y) / 2f, //y
                                  p.start.z * (1 - p.maxTime) + p.end.z * p.maxTime); //z

            p.projBody.LookAt(new Vector3(p.start.x * (1 - p.maxTime * 1.1f) + p.end.x * p.maxTime * 1.1f,  //x
                                  (1 - p.maxTime * 1.1f) * (p.maxTime * 1.1f) * (archHeight) + (p.start.y + p.end.y) / 2f, //y
                                  p.start.z * (1 - p.maxTime * 1.1f) + p.end.z * p.maxTime * 1.1f)); //z
        }
    }
}
//public virtual void Setup(Vector3 targetedPosition) {
//    //if (!isServer)
//    //    Destroy(this);
//    targPos = targetedPosition;
//    startPos = transform.position;
//    float distance = Helpers.Vector3Distance(transform.position, targPos);
//    if(distance > range) {
//        airTime *= distance / range;
//        archHeight*= distance / range;
//    }
//    layer = 1 << LayerMask.NameToLayer("Unit");
//}

//private void FixedUpdate() {
//    if(targPos != null) {
//        timer += Time.deltaTime / airTime;
//        t = timer / airTime;

//        transform.position = new Vector3(startPos.x * (1 - t) + targPos.x * t,  //x
//                              (1 - t) * (t) * (archHeight) + (startPos.y + targPos.y) / 2f, //y
//                              startPos.z * (1 - t) + targPos.z * t); //z

//        transform.LookAt(new Vector3(startPos.x * (1 - t * 1.1f) + targPos.x * t * 1.1f,  //x
//                              (1 - t * 1.1f) * (t * 1.1f) * (archHeight) + (startPos.y + targPos.y) / 2f, //y
//                              startPos.z * (1 - t * 1.1f) + targPos.z * t * 1.1f)); //z
//        if (transform.position.y <= 0) {
//            if (explosive)
//                Explode();
//            NetworkServer.Destroy(gameObject);
//        }
//        if (!explosive) {
//            foreach (Vector3 direction in directions) {
//                Debug.DrawRay(transform.position, transform.TransformDirection(direction) * rayLength, Color.yellow/*, 0.1f*/);
//                Ray ray = new Ray(transform.position, transform.TransformDirection(direction));
//                if (Physics.Raycast(ray, out RaycastHit hit, rayLength) && hit.collider.isTrigger == false) {
//                    if (hit.collider.transform.TryGetComponent(out Team t)) {
//                        if (t.GetTeam() == GetComponent<Team>().GetTeam())
//                            return;
//                    }
//                    if (explosive) {
//                        Explode();
//                        return;
//                    }
//                    if (hit.collider.transform.TryGetComponent(out Health health)) {
//                        health.Damage(damage);
//                        NetworkServer.Destroy(gameObject);
//                    }
//                    break;
//                }
//            }
//        }
//    }
//}

//private void Explode() {
//    if (NetworkServer.active) {
//        RaycastHit[] hits = Physics.SphereCastAll(transform.position, expRad, transform.forward * 0.01f, 0, layer, QueryTriggerInteraction.Ignore);

//        GameObject obj = Instantiate(particles, transform.position, transform.rotation);
//        NetworkServer.Spawn(obj);
//        obj.transform.position = transform.position;
//        obj.transform.position = transform.position;
//        obj.transform.parent = null;

//        if (hits.Length > 0) {
//            foreach (RaycastHit hit in hits) {
//                if (hit.transform.TryGetComponent(out Team t) && hit.transform.TryGetComponent(out Health health)) {
//                    if (t.GetTeam() == GetComponent<Team>().GetTeam())
//                        return;
//                    health.Damage(damage);
//                }
//            }
//        }
//        NetworkServer.Destroy(gameObject);
//    }
//}