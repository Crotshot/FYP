using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Mirror;
/*
 Controls the ownership of buildings and changes them when captured by enemy minions
 Buildings take X minions to neutralize and then X minions again to capture
*/

public class StructureCaptureState : NetworkBehaviour
{
    [SerializeField] int minionsToCapture;
    [SerializeField] TMP_Text capturetext;
    [SerializeField] Image textBackground;
    [SerializeField] GameObject entryPoint;
    private int charges = 0, currentTeam = 0;
    bool neutral = true;

    public UnityEvent<int> teamChanged;

    private void Start() {
        if (isServer) {
            StartCoroutine("SetupEntry");
        }
        if (teamChanged == null)
            teamChanged = new UnityEvent<int>();
    }

    IEnumerator SetupEntry() {
        yield return new WaitForSeconds(5f);
        GameObject capPoint = Instantiate(entryPoint, null);
        NetworkServer.Spawn(capPoint);
        capPoint.GetComponent<EntryTrigger>().entryTouched.AddListener(MinionEntered);
        capPoint.transform.position = transform.GetChild(0).position;
        capPoint.transform.rotation = transform.GetChild(0).rotation;
    }

    private void MinionEntered(GameObject minion) {
        int team = minion.GetComponent<Team>().GetTeam();
        Color col = minion.GetComponent<Team>().GetTeamColor();

        if (neutral) {
            if (team == currentTeam) {
                if (charges < minionsToCapture) {
                    charges++;
                    ReflectCharges(charges);
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
                    ReflectCharges(charges);
                    currentTeam = team;
                    if (charges == 0) {
                        textBackground.color = Color.white;
                        ReflectColour(1, 1, 1, 1);
                    }
                    else {
                        textBackground.color = col;
                        ReflectColour(col.r, col.g, col.b, col.a);
                    }
                }
            }
        }
        else {
            if (team == currentTeam) {
                if (charges < minionsToCapture) {
                    charges++;
                    ReflectCharges(charges);
                    Destroy(minion);
                    if (charges == minionsToCapture) {
                        neutral = false;
                    }
                }
            }
            else {
                charges--;
                ReflectCharges(charges);
                Destroy(minion);
                if (charges == 0) {
                    neutral = true;
                    currentTeam = 0;
                }
            }
        }

        if (charges == 0) {
            textBackground.color = Color.white;
            ReflectColour(1, 1, 1, 1);
        }
        capturetext.SetText(charges.ToString());
    }

    [ClientRpc]
    private void ReflectCharges(int charges) {
        capturetext.SetText(charges.ToString());
    }

    [ClientRpc]
    private void ReflectColour(float r, float g, float b, float a) {
        textBackground.color = new Color(r,g,b,a);
    }

    public int getOwningTeam() {
        return currentTeam;
    }
}
