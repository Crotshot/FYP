using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion_Melee : Minion_Attack{
    //Attack time is the time an attack is active
    //Attack cooldown is the time an attack is inactive
    [SerializeField] float damage, rayLength;
    bool attackHit;//Can only hit 1 enemy per attack
    [SerializeField] Vector3[] directions;

    private void Start() {
        if (!isServer)
            Destroy(this);
        animatedWeapon.localPosition = animatedTimings[0].pos;
        animatedWeapon.localScale = animatedTimings[0].scale;
        animatedWeapon.localEulerAngles = animatedTimings[0].localEuler;
    }

    private void FixedUpdate() {
        AnimatedAttack(RayCheck);
    }

    private void RayCheck() {
        if (!attackHit) {
            foreach (Vector3 direction in directions) {
                Debug.DrawRay(animatedWeapon.position, animatedWeapon.TransformDirection(direction) * rayLength, Color.green/*, 0.1f*/);
                Ray ray = new Ray(animatedWeapon.position, animatedWeapon.TransformDirection(direction));
                if (Physics.Raycast(ray, out RaycastHit hit, rayLength) && hit.collider.isTrigger == false) {
                    if (hit.collider.transform.TryGetComponent(out Team t) && hit.collider.transform.TryGetComponent(out Health health)) {
                        if (t.GetTeam() == GetComponent<Team>().GetTeam())
                            continue;
                        health.Damage(damage);
                        attackHit = true;
                        return;
                    }
                }
            }
        }
    }

    public override void Attack(){
        if (attacking)
            return;
        attackTimer = 0;
        attackHit = false;
        attacking = true;
        index = 1;
    }
}
