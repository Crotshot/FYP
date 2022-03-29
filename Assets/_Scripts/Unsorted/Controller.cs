using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Controller : NetworkBehaviour {

    [SerializeField] [Min(0)] protected float characterSpeed = 5f, rotSpeed = 5f;
    [SerializeField] protected float actualSpeed, modifierSpeed = 1, modifierSlow = 1;
    [SyncVar]protected bool stunned = false;

    virtual public void Setup() {
        Debug.Log("Setting up controller");
        if(isServer)
            GetComponent<Status>().Init();
    }

    protected virtual void FixedUpdate() {
        if (stunned)
            return;
    }

    public virtual void EffectStart(string effect, float value) {
        if (effect.Equals("Stun")) {
            stunned = true;
            return;
        }
        else if (effect.Equals("Speed")) {
            modifierSpeed = value + 1;
            actualSpeed = characterSpeed * modifierSpeed * modifierSlow;
            return;
        }
        else if (effect.Equals("Slow")) {
            modifierSlow = 1 - value;
            actualSpeed = characterSpeed * modifierSpeed * modifierSlow;
            return;
        }
        Debug.Log("Controller for " + gameObject.name + " does not contain effect: " + effect);
    }

    public virtual void EffectEnd(string effect) {
        if (effect.Equals("Stun")) {
            stunned = false;
            return;
        }
        else if (effect.Equals("Speed")) {
            modifierSpeed = 1;
            actualSpeed = characterSpeed * modifierSpeed * modifierSlow;
            return;
        }
        else if (effect.Equals("Slow")) {
            modifierSlow = 1;
            actualSpeed = characterSpeed * modifierSpeed * modifierSlow;
            return;
        }
        Debug.Log("Controller for " + gameObject.name + " does not contain effect: " + effect);
    }
}