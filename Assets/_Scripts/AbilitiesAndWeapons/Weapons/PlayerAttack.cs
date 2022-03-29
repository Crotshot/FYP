using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class PlayerAttack : NetworkBehaviour {

    public UnityEvent weaponFired;

    virtual protected void Start() {
        if (!hasAuthority)
            return;
        if (weaponFired == null)
            weaponFired = new UnityEvent();
        GetComponent<PlayerController>().atk.AddListener(Attack);
        Debug.Log("Listener for Weapon set");
    }

    virtual public void Attack() {

    }
}