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
    PlayerMinions pM;

    [SerializeField] bool debug;

    int minionsQueued = 0;
    bool coRoutineActive;

    public void Setup(Vector3 location, Vector3 rotation) {
        RpcSetup(location, rotation);
    }

    [ClientRpc]
    private void RpcSetup(Vector3 location, Vector3 rotation) {
        transform.parent = null;
        transform.position = location;
        transform.eulerAngles = rotation;
        if (debug)
            Debug.Log("Setting up minion spawner");
        int team = 0;
        if (transform.position.z > 0)
            team = 1;
        else
            team = 2;
        if (debug)
            Debug.Log("Team: " + team);
        GetComponent<Team>().SetTeam(team);
        Color col = FindObjectOfType<Colors>().GetTeamColor(team);
        GetComponent<Team>().SetTeamColor(col.a, col.g, col.b, col.a);

        cost.text = minionCost.ToString();
        background.color = col;

        pool = FindObjectOfType<MinionPool>();
        mm = FindObjectOfType<MinionManager>();

        foreach (PlayerCurrency p in FindObjectsOfType<PlayerCurrency>()) {
            if (p.GetComponent<Team>().GetTeam() == GetComponent<Team>().GetTeam()) {
                pC = p;
                pM = p.GetComponent<PlayerMinions>();
                break;
            }
        }
    }

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
            pM.AddFollower(SpawnMinion(minion_1, true, mType).GetComponent<MinionController>());
            yield return new WaitForSeconds(0.21f);
            minionsQueued--;
        }
        coRoutineActive = false;
    }


}