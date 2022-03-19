using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using Mirror;
using Helpers = Crotty.Helpers.StaticHelpers;

public class PlayerController : Controller {

    [SerializeField][Min(0)] protected float characterSpeed = 5f, rotSpeed = 5f;
    protected Inputs inputs;
    protected Rigidbody rb;

    public UnityEvent ab1, ab2, ab3, atk;
    protected bool ready = false;

    virtual public void Setup() {
        if (hasAuthority) {
            transform.GetChild(0).GetComponent<Camera_Follower>().Setup();
            inputs = FindObjectOfType<Inputs>();

            if (ab1 == null)
                ab1 = new UnityEvent();
            if (ab2 == null)
                ab2 = new UnityEvent();
            if (ab3 == null)
                ab3 = new UnityEvent();
            if (atk == null)
                atk = new UnityEvent();

            Destroy(GetComponent<WorldSpaceHealthBar>());
            FindObjectOfType<UI>().Setup(GetComponent<PlayerHealth>(), GetComponent<PlayerCurrency>());
            GetComponent<Interactor>().Setup();
            rb = GetComponent<Rigidbody>();
        }
        else{
            Destroy(transform.GetChild(0).gameObject);
            GetComponent<WorldSpaceHealthBar>().SetupDelayed();
            Destroy(this);
        }
    }

    virtual protected void FixedUpdate() {
        if (!ready || !stunned)
            return;

        Vector3 input = inputs.GetMovementInput();
        if (input.x != 0)
            transform.RotateAround(transform.position, Vector3.up, rotSpeed * Time.deltaTime * input.x);
        if (input.z != 0) {
            Vector3 currentVelocity = transform.forward * Time.deltaTime * characterSpeed * input.z * 50f;
            currentVelocity = Helpers.Vector3Clamp(currentVelocity, -characterSpeed, characterSpeed);
            rb.velocity = currentVelocity;
        }
    }

    protected void Update() {
        if (!ready || !stunned)
            return;

        if(inputs.GetAbility1Input() > 0) {
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
    }

    public void Release() {
        ready = true;
    }

    public float GetCharacterSpeed() {
        return characterSpeed;
    }

    public void SetCharacterSpeed(float speed) {
        characterSpeed = speed; // Changes base speed of character
    }

    override public void EffectStart(string effect) {
        if (effect.Equals("Stun")) {
            stunned = true;
            return;
        }
        base.EffectStart(effect);
    }

    override public void EffectEnd(string effect) {
        if (effect.Equals("Stun")) {
            stunned = false;
            return;
        }
        base.EffectEnd(effect);
    }
}