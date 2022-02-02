using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GoldMine : NetworkBehaviour
{
    [SerializeField] int goldPerTick = 1;
    [SerializeField] float timerPerTick = 2;
    float tickTimer;
    PlayerCurrency pC;

    void Start()
    {
        if (isServer) {
            GetComponent<StructureCaptureState>().teamChanged.AddListener(TeamChanged);
        }
        else {
            Destroy(this);
        }
    }

    void TeamChanged(int i) {
        foreach (PlayerCurrency pC1 in FindObjectsOfType<PlayerCurrency>()) {
            if(pC1.GetComponent<Team>().GetTeam() == i) {
                pC = pC1;
                break;
            }
        }
    }

    private void FixedUpdate() {
        if (pC == null)
            return;

        tickTimer -= Time.deltaTime;
        if(tickTimer <= 0) {
            tickTimer = timerPerTick;
            pC.AddShinies(goldPerTick);
        }
    }
}
