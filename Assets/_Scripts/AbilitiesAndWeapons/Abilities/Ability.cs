using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class Ability : NetworkBehaviour
{
    public int input; //1 2 or 3
    [SerializeField] protected float coolDown;
    protected float coolDownTimer;

    /// <summary>
    /// Sets ability listeners for the player controller,. The added componenet desipte existing everywhere is local
    /// therefore to access, it must be done using net id's
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abilityCast"></param>
    protected void SetUp<T>(UnityEngine.Events.UnityAction abilityCast) where T : Component, new() {
        gameObject.AddComponent<T>(); //Always attach effect component
        if (hasAuthority) {
            if (input == 1) {
                GetComponent<PlayerController>().ab1.AddListener(abilityCast);
               // Debug.Log("Object: " + gameObject.name + "listening for ability " + input);
            }
            else if (input == 2) {
                GetComponent<PlayerController>().ab2.AddListener(abilityCast);
               // Debug.Log("Object: " + gameObject.name + "listening for ability " + input);
            }
            else if (input == 3) {
                GetComponent<PlayerController>().ab3.AddListener(abilityCast);
                //Debug.Log("Object: " + gameObject.name + "listening for ability " + input);
            }
        }
    }

    //Might infact be better as we cannot instantiate objects across the server
    protected void SetUp(UnityEngine.Events.UnityAction abilityCast) {
        if (hasAuthority) {
            if (input == 1) {
                GetComponent<PlayerController>().ab1.AddListener(abilityCast);
                //Debug.Log("Object: " + gameObject.name + "listening for ability " + input);
            }
            else if (input == 2) {
                GetComponent<PlayerController>().ab2.AddListener(abilityCast);
               // Debug.Log("Object: " + gameObject.name + "listening for ability " + input);
            }
            else if (input == 3) {
                GetComponent<PlayerController>().ab3.AddListener(abilityCast);
                //Debug.Log("Object: " + gameObject.name + "listening for ability " + input);
            }
        }
    }

    protected void CoolDown(float time) {
        if(coolDownTimer > 0) {
            coolDownTimer -= time;
        }
    }

    protected bool AbilityUsed() {
        if(coolDownTimer <= 0) {
            coolDownTimer = coolDown;
            return true;
        }
        return false;
    }

    protected void AbilityDamage(Transform form, float damageToDeal) {
        if (form.TryGetComponent<Health>(out Health health) && form.TryGetComponent<Team>(out Team team)) {
            if (team.GetTeam() != GetComponent<Team>().GetTeam()) {
                health.Damage(damageToDeal);
            }
        }
    }

    public float GetCoolDownRatio() {
        if (coolDownTimer <= 0)
            return 0;
        return coolDownTimer / coolDown;
    }

    public float GetCoolDownTimer() {
        if (coolDownTimer <= 0)
            return 0;
        return coolDownTimer;
    }
}
