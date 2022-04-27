using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using Helpers = Crotty.Helpers.StaticHelpers;

public class Minion_Attack : NetworkBehaviour
{
    [SerializeField] protected float attackTime, attackCooldownTime;
    [SerializeField] protected AnimatedWeaponTiming[] animatedTimings;
    [SerializeField] protected Transform animatedWeapon;
    protected float attackTimer;
    protected bool attacking;
<<<<<<< Updated upstream
    protected int index;
    public virtual void Attack() { }
=======
    protected int index; 
    [ClientRpc]
    public virtual void RpcAttack() { }
>>>>>>> Stashed changes

    protected bool stunned;

    protected void AnimatedAttack(Action act) {
        if (attacking) { //Atacking
            float W = animatedTimings[index].time - animatedTimings[index - 1].time;
            float percentage = (attackTimer - animatedTimings[index-1].time) / W;

            if (animatedTimings[index].pos != animatedTimings[index - 1].pos) {
                animatedWeapon.localPosition = Helpers.Vector3Follow(animatedTimings[index - 1].pos, animatedTimings[index].pos, percentage);
            }
            if (animatedTimings[index].scale != animatedTimings[index - 1].scale) {
                animatedWeapon.localScale = Helpers.Vector3Follow(animatedTimings[index - 1].scale, animatedTimings[index].scale, percentage);
            }
            if (animatedTimings[index].localEuler != animatedTimings[index - 1].localEuler) {
                animatedWeapon.localEulerAngles = Helpers.Vector3Follow(animatedTimings[index - 1].localEuler, animatedTimings[index].localEuler, percentage);
            }

<<<<<<< Updated upstream
            if (attackTimer <= attackTime) {
                //Check for damage
                act();
=======
            if (isServer && attackTimer <= attackTime) {
                act();//Check for damage only on server
>>>>>>> Stashed changes
            }
            else if (attackTimer >= attackTime + attackCooldownTime) {
                attacking = false;
                animatedWeapon.localPosition = animatedTimings[animatedTimings.Length - 1].pos;
                animatedWeapon.localScale = animatedTimings[animatedTimings.Length - 1].scale;
                animatedWeapon.localEulerAngles = animatedTimings[animatedTimings.Length - 1].localEuler;
            }

<<<<<<< Updated upstream
            ReflectWeapon(animatedWeapon.localPosition, animatedWeapon.localEulerAngles, animatedWeapon.localScale);
=======
            //ReflectWeapon(animatedWeapon.localPosition, animatedWeapon.localEulerAngles, animatedWeapon.localScale);
>>>>>>> Stashed changes


            attackTimer += Time.deltaTime;
            if (animatedTimings[index].time <= attackTimer) {
                index++;
                if (index >= animatedTimings.Length)
                    attacking = false;
            }
        }
    }

    public void isStun(bool stun) {
        stunned = stun;
    }

<<<<<<< Updated upstream
    [ClientRpc]
    public void ReflectWeapon(Vector3 pos, Vector3 rot, Vector3 scale) {
        animatedWeapon.localPosition = pos;
        animatedWeapon.localEulerAngles = rot;
        animatedWeapon.localScale = scale;
    }
=======
    //[ClientRpc]
    //public void ReflectWeapon(Vector3 pos, Vector3 rot, Vector3 scale) {
    //    animatedWeapon.localPosition = pos;
    //    animatedWeapon.localEulerAngles = rot;
    //    animatedWeapon.localScale = scale;
    //}

    //[ClientRpc]
    //public void RpcAttack() {
    //    Attack();
    //}
>>>>>>> Stashed changes
}

[System.Serializable]
public class AnimatedWeaponTiming {
    [SerializeField] public Vector3 pos, localEuler, scale;//Transform of weapon
    [SerializeField] public float time; //When attack timer = time move onto interpolating next AnimatedWeaponTiming
}