using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class BasePoint : ControlPoint {
    [SerializeField] float basePointModifier = 100, defenderModifier = 0.1f;

    protected override void Start() {
        base.Start();
        if (isServer) {
            captured.AddListener(RpcSetupBasePoint);
        }
    }

    [ClientRpc]
    private void RpcSetupBasePoint() {
        if (isServer) {
            captured.RemoveListener(RpcSetupBasePoint);
            neutralised.AddListener(RpcGameOver);
        }
        chargesToCapture *= basePointModifier;
        currentCharges = chargesToCapture;
        friendlyModifier = defenderModifier;
    }

    [ClientRpc]
    private void RpcGameOver() {
        FindObjectOfType<UI>().ShowGameOver(currentTeam == 1 ? 2 : 1);
    }
}
