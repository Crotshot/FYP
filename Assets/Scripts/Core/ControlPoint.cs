using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Mirror;

public class ControlPoint : NetworkBehaviour
{

    [SerializeField] private int assignedMinions_1, assignedMinions_2, maxAssignedMinions = 10;

    [SerializeField] private float chargesToCapture;
    [SerializeField] Image captureFillImage;
    [SerializeField] bool basePoint;

    [SerializeField] private float charges;
    [SerializeField] int currentTeam = 0;
    [SerializeField] Color fillColor;

    List<Transform> trackedTransforms = new List<Transform>();
    List<int> teams = new List<int>();
    private enum TeamState { Neutral, Captured}
    TeamState capState = TeamState.Neutral;

    public UnityEvent captured;
    private void Start() {
        if (isServer && captured == null)
            captured = new UnityEvent();
    }

    private void FixedUpdate() {
        if (isServer) {
            float teamCharges = 0;
            int teamLean = 0;
            foreach (Transform form in trackedTransforms) {
                if (form == null) {
                    trackedTransforms.Remove(form);
                    continue;
                }

                if (form.GetComponent<Team>().GetTeam() == teamLean) {
                    if (form.tag.Equals("Player")) teamCharges += 10f * Time.deltaTime; else teamCharges += 1f * Time.deltaTime;
                }
                else {
                    if (form.tag.Equals("Player")) teamCharges -= 10f * Time.deltaTime; else teamCharges -= 1f * Time.deltaTime;
                    if (teamCharges <= 0) {
                        teamCharges *= -1;
                        teamLean = form.GetComponent<Team>().GetTeam();
                        fillColor = form.GetComponent<Team>().GetTeamColor();
                    }
                }
            }
            if (teamCharges == 0)
                return;
            Capturing(teamCharges, teamLean, fillColor);
            RPCReflectFill(charges);
        }
    }

    [ClientRpc]
    private void RPCReflectFill(float serverCharges) {
        charges = serverCharges;
        captureFillImage.fillAmount = charges / chargesToCapture;
    }

    [ClientRpc]
    private void RPCReflectOwnerShip(int team, float r, float g, float b, float a) {
        fillColor = new Color(r,g,b,a);
        captureFillImage.color = fillColor;
        currentTeam = team;
    }

    private void Capturing(float teamC, int team, Color col) {
        if(capState == TeamState.Neutral) {
            if(team == currentTeam) {
                charges += teamC;
                if (charges >= chargesToCapture) {
                    charges = chargesToCapture;
                    captured?.Invoke();
                    capState = TeamState.Captured;
                    if (basePoint) {
                        chargesToCapture *= 100f;
                        charges = chargesToCapture;
                    }
                }
            }
            else {
                charges -= teamC;
                if (charges <= 0) {
                    charges *= -1;
                    currentTeam = team;
                    captureFillImage.color = col;
                    RPCReflectOwnerShip(team, col.r, col.g, col.b, col.a);
                }
            }
        }
        else {
            if (team == currentTeam) {
                if (!basePoint) {
                    charges += teamC;
                    if (charges >= chargesToCapture) {
                        charges = chargesToCapture;
                        captured?.Invoke();
                    }
                }
            }
            else {
                charges -= teamC;
                if (charges <= 0) {
                    charges *= -1;
                    capState = TeamState.Neutral;
                    currentTeam = team;
                    captureFillImage.color = col;
                     RPCReflectOwnerShip(team, col.r, col.g, col.b, col.a);
                    if (basePoint) {
                        print("Team: " + currentTeam + " has won the match!");
                    }
                }
            }
        }

        captureFillImage.fillAmount = charges/chargesToCapture;
    }

    //Player, minion
    private void OnTriggerEnter(Collider other) {
        if(other.tag.Equals("minion") || other.tag.Equals("Player") && !trackedTransforms.Contains(other.transform)) {
            trackedTransforms.Add(other.transform);
            if (!teams.Contains(other.GetComponent<Team>().GetTeam())) {
                teams.Add(other.GetComponent<Team>().GetTeam());
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag.Equals("minion") || other.tag.Equals("Player") && trackedTransforms.Contains(other.transform)) {
            trackedTransforms.Remove(other.transform);
        }
    }


    public int GetTeam() {
        if (capState == TeamState.Captured) {
            return currentTeam;
        }
        else {
            return 0;
        }
    }

    public bool GetBasePoint() {
        return basePoint;
    }

    public int GetCurrentMinions(int team) {
        if (team == 1) {
            return assignedMinions_1;
        }
        else if (team == 2) {
            return assignedMinions_2;
        }
        else {
            return 0;
        }
    }

    public int GetMaxMinions() {
        return maxAssignedMinions;
    }

    public void SetMaxMinions(int newMax) {
        maxAssignedMinions = newMax;
    }

    public void AddMinion(int team) {
        if(team == 1) {
            assignedMinions_1++;
        }
        else if(team == 2) {
            assignedMinions_2++;
        }
    }

    public void RemoveMinion(int team) {
        if (team == 1) {
            assignedMinions_1--;
        }
        else if (team == 2) {
            assignedMinions_2--;
        }
    }
}