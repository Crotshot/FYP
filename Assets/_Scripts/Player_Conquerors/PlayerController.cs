using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using Mirror;
using Helpers = Crotty.Helpers.StaticHelpers;

public class PlayerController : Controller {

    protected Inputs inputs;
    protected Rigidbody rb;

    public UnityEvent ab1, ab2, ab3, atk;
    protected bool ready = false;

    override public void Setup() {
        base.Setup();
        //GetComponent<WorldSpaceHealthBar>().SetupDelayed();
        if (hasAuthority) {
            transform.GetChild(0).GetComponent<Camera_Follower>().Setup();
            inputs = FindObjectOfType<Inputs>();
            actualSpeed = characterSpeed;

            if (ab1 == null)
                ab1 = new UnityEvent();
            if (ab2 == null)
                ab2 = new UnityEvent();
            if (ab3 == null)
                ab3 = new UnityEvent();
            if (atk == null)
                atk = new UnityEvent();

            //Destroy(GetComponent<WorldSpaceHealthBar>());
            FindObjectOfType<UI>().Setup(GetComponent<PlayerHealth>(), GetComponent<PlayerCurrency>());
            GetComponent<Interactor>().Setup();
            rb = GetComponent<Rigidbody>();
        }
        else{
            Destroy(transform.GetChild(0).gameObject);
            Destroy(this);
        }
    }

    override protected void FixedUpdate() {
        if (!ready)
            return;
        base.FixedUpdate();

        if (inputs.GetAbility1Input() > 0) {
            ab1?.Invoke();
        }
        if (inputs.GetAbility2Input() > 0) {
            ab2?.Invoke();
        }
        if (inputs.GetAbility3Input() > 0) {
            ab3?.Invoke();
        }
        if (inputs.GetAttackInput() > 0) {
            atk?.Invoke();
        }

        Vector3 input = inputs.GetMovementInput();
        if (input.x != 0)
            transform.RotateAround(transform.position, Vector3.up, rotSpeed * Time.deltaTime * input.x);
        if (input.z != 0) {
            Vector3 currentVelocity = transform.forward * Time.deltaTime * actualSpeed * input.z * force;
            currentVelocity = Helpers.Vector3Clamp(currentVelocity, -actualSpeed, actualSpeed);
            rb.velocity = currentVelocity;
        }
    }

    public void Release() {
        ready = true;
    }

    public float GetCharacterSpeed() {
        return characterSpeed;
    }

    public void SetCharacterSpeed(float speed) {
        characterSpeed = speed; // Changes base speed of character
        actualSpeed = characterSpeed * modifierSpeed * modifierSlow;
    }

    override public void EffectStart(string effect, float value) {
        base.EffectStart(effect, value);
    }

    override public void EffectEnd(string effect) {
        base.EffectEnd(effect);
    }
}