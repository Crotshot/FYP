using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
using Helpers = Crotty.Helpers.StaticHelpers;
using UnityEngine.UI;
using TMPro;
public class MinionSumoner : MinionSpawner_Base { //Can only exist on server
    [SerializeField] int minionCost;
    [SerializeField] string mType;
    [SerializeField] Image background;
    [SerializeField] TMP_Text cost;
    PlayerCurrency pC;

    int minionsQueued = 0;
    bool coRoutineActive;

    public void Setup(PlayerCurrency[] pCs) {
        int team = 0;
        if (transform.position.z > 0)
            team = 1;
        else
            team = 2;
        GetComponent<Team>().SetTeam(team);
        Color col = FindObjectOfType<Colors>().GetTeamColor(team);
        GetComponent<Team>().SetTeamColor(col.a, col.g, col.b, col.a);

        cost.text = minionCost.ToString();
        background.color = col;

        if (!isServer)
            Destroy(this);

        pool = FindObjectOfType<MinionPool>();
        mm = FindObjectOfType<MinionManager>();

        foreach (PlayerCurrency p in pCs) {
            if (p.GetComponent<Team>().GetTeam() == GetComponent<Team>().GetTeam()) {
                pC = p;
                break;
            }
        }
    }

    //TEST CODE
    public bool summon;

    private void Update() {
        if (summon) {
            summon = false;
            Summon();
        }
    }

    //
    public void Summon() {
        if (pC.SpendShinies(minionCost)) {
            minionsQueued++;
            if (!coRoutineActive) {
                StartCoroutine("MinionSummoningQue");
            }
        }
    }

    IEnumerator MinionSummoningQue() {
        coRoutineActive = true;
        while (minionsQueued > 0) {
            SpawnMinion(minion_1, true, mType);
            yield return new WaitForSeconds(0.66f);
            minionsQueued--;
        }
        coRoutineActive = false;
    }

    public void HighLight() {
        //DO LATER
    }
}