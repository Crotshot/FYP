using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Ability : NetworkBehaviour
{
    public int input; //1 2 or 3
    [SerializeField] protected float coolDown;
    protected float coolDownTimer;

    protected void SetUp(UnityEngine.Events.UnityAction cast) {
        if(input == 1) {
            GetComponent<PlayerController>().ab1.AddListener(cast);
            Debug.Log("Object: " + gameObject.name + "listening for ability " + input);
        }
        else if(input == 2) {
            GetComponent<PlayerController>().ab2.AddListener(cast);
            Debug.Log("Object: " + gameObject.name + "listening for ability " + input);
        }
        else if(input == 3) {
            GetComponent<PlayerController>().ab3.AddListener(cast);
            Debug.Log("Object: " + gameObject.name + "listening for ability " + input);
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
}
