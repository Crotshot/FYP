using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
/*
 Controls the ownership of buildings and changes them when captured by enemy minions
 Buildings take X minions to neutralize and then X minions again to capture
*/

public class StructureCaptureState : MonoBehaviour
{
    [SerializeField] int minionsToCapture;
    [SerializeField] TMP_Text capturetext;
    [SerializeField] Image textBackground;
    private int currentMinions, owningTeam;
    bool neutral = true;

    public UnityEvent<int> teamChanged;

    private void Start() {
        transform.GetChild(0).GetComponent<EntryTrigger>().entryTouched.AddListener(MinionEntered);
        currentMinions = 0;

        if (teamChanged == null)
            teamChanged = new UnityEvent<int>();
    }

    private void MinionEntered(GameObject minion) {
        int team = minion.GetComponent<Team>().GetTeam();
        Color col = minion.GetComponent<Team>().GetTeamColor();

        if (neutral) {
            if(currentMinions == 0) {
                owningTeam = team;
                currentMinions++;
                Destroy(minion);
            }
            else if(team == owningTeam) {
                currentMinions++;
                if (currentMinions == minionsToCapture) {
                    neutral = false;
                }
                Destroy(minion);
            }
            else {
                currentMinions--;
                Destroy(minion);
                if (currentMinions == 0) {
                }
            }
        }
        else if(team == owningTeam && currentMinions < minionsToCapture) {
            Destroy(minion);
            currentMinions++;
        }
        else if(team != owningTeam){
            currentMinions--;
            Destroy(minion);
            if (currentMinions == 0) {
                neutral = true;
            }
        }

        capturetext.SetText(currentMinions.ToString());
        if (currentMinions == 0)
            textBackground.color = Color.white;
        else
            textBackground.color = col;
    }

    public int getOwningTeam() {
        return owningTeam;
    }
}
