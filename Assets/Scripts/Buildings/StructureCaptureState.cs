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
    private int charges = 0, currentTeam = 0;
    bool neutral = true;

    public UnityEvent<int> teamChanged;

    private void Start() {
        transform.GetChild(0).GetComponent<EntryTrigger>().entryTouched.AddListener(MinionEntered);
        if (teamChanged == null)
            teamChanged = new UnityEvent<int>();
    }

    private void MinionEntered(GameObject minion) {
        int team = minion.GetComponent<Team>().GetTeam();
        Color col = minion.GetComponent<Team>().GetTeamColor();

        if (neutral) {
            if (team == currentTeam) {
                if (charges < minionsToCapture) {
                    charges++;
                    Destroy(minion);
                    if (charges == minionsToCapture) {
                        neutral = false;
                    }
                }
            }
            else {
                charges--;
                Destroy(minion);
                if (charges <= 0) {
                    charges *= -1;
                    currentTeam = team;
                    if (charges == 0)
                        textBackground.color = Color.white;
                    else
                        textBackground.color = col;
                }
            }
        }
        else {
            if (team == currentTeam) {
                if (charges < minionsToCapture) {
                    charges++;
                    Destroy(minion);
                    if (charges == minionsToCapture) {
                        neutral = false;
                    }
                }
            }
            else {
                charges--;
                Destroy(minion);
                if (charges == 0) {
                    neutral = true;
                    currentTeam = 0;
                }
            }
        }

        if (charges == 0)
            textBackground.color = Color.white;
        capturetext.SetText(charges.ToString());
    }

    public int getOwningTeam() {
        return currentTeam;
    }
}
