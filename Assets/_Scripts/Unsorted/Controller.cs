using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Controller : NetworkBehaviour {

    [SyncVar]protected bool stunned = false;

    virtual public void Setup() {
        Debug.Log("Setting up controller");
        if(isServer)
            GetComponent<Status>().Init();
    }

    public virtual void EffectStart(string effect) {
        Debug.Log("Controller for " + gameObject.name + " does not contain effect: " + effect);
    }

    public virtual void EffectEnd(string effect) {
        Debug.Log("Controller for " + gameObject.name + " does not contain effect: " + effect);
    }
}