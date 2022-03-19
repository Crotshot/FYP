using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Mirror;

public class ControlPoint : NetworkBehaviour
{
    [SerializeField] ControlPoint mid, SE, NE, SW, NW;
    [SerializeField] private int assignedMinions_1, assignedMinions_2, maxAssignedMinions = 10;

    [SerializeField] private float chargesToCapture;
    [SerializeField] Image captureFillImage;
    [SerializeField] bool basePoint;

    [SerializeField] private float charges;
    [SerializeField] int currentTeam = 0;
    [SerializeField] Color fillColor;

    [SerializeField] float syncGap = 1f;
    float syncTimer;

    List<Transform> trackedTransforms = new List<Transform>();
    List<int> teams = new List<int>();
    private enum TeamState { Neutral, Captured}
    TeamState capState = TeamState.Neutral;

    bool setUp, notCapped = true;

    public UnityEvent captured;
    public void Setup() {
        if (isServer && captured == null)
            captured = new UnityEvent();
        setUp = true;
        syncTimer = syncInterval;
    }

    private void FixedUpdate() {
        if (!setUp)
            return;
        if (isServer) {
            float teamCharges = 0;
            int teamLean = 0;
            for (int i = trackedTransforms.Count -1; i > -1; i--) {
                if (trackedTransforms[i] == null) {
                    trackedTransforms.Remove(trackedTransforms[i]);
                    continue;
                }

                if (trackedTransforms[i].GetComponent<Team>().GetTeam() == teamLean) {
                    if (trackedTransforms[i].tag.Equals("Player")) teamCharges += 10f * Time.deltaTime; else teamCharges += 1f * Time.deltaTime;
                }
                else {
                    if (trackedTransforms[i].tag.Equals("Player")) teamCharges -= 10f * Time.deltaTime; else teamCharges -= 1f * Time.deltaTime;
                    if (teamCharges <= 0) {
                        teamCharges *= -1;
                        teamLean = trackedTransforms[i].GetComponent<Team>().GetTeam();
                        fillColor = trackedTransforms[i].GetComponent<Team>().GetTeamColor();
                    }
                }
            }

            if (teamCharges == 0)
                return;
            Capturing(teamCharges, teamLean, fillColor);

            if(trackedTransforms.Count > 0)
                if(syncTimer > 0) {
                    syncTimer -= Time.deltaTime;
                }
                else {
                    RPCReflectFill(charges);
                    syncTimer = syncInterval;
                }
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
                if (basePoint && !notCapped) {
                    if(mid.GetTeam() != 0 && (mid.GetTeam() == SE.GetTeam() || mid.GetTeam() == NE.GetTeam()) && (mid.GetTeam() == SW.GetTeam() || mid.GetTeam() == NW.GetTeam())) {
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
                    charges -= teamC;
                    if (charges <= 0) {
                        charges *= -1;
                        currentTeam = team;
                        captureFillImage.color = col;
                        RPCReflectOwnerShip(team, col.r, col.g, col.b, col.a);
                        notCapped = false;
                    }
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
                        Debug.Log("Team: " + currentTeam + " has won the match!");
                        RpcGameOver(currentTeam);
                    }
                }
            }
        }

        captureFillImage.fillAmount = charges/chargesToCapture;
    }

    [ClientRpc]
    private void RpcGameOver(int t) {
        FindObjectOfType<UI>().ShowGameOver(t);
    }

    //Player, minion
    private void OnTriggerEnter(Collider other) {
        if (!setUp)
            return;
        if (other.tag.Equals("minion") || other.tag.Equals("Player") && !trackedTransforms.Contains(other.transform)) {
            trackedTransforms.Add(other.transform);
            if (!teams.Contains(other.GetComponent<Team>().GetTeam())) {
                teams.Add(other.GetComponent<Team>().GetTeam());
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!setUp)
            return;
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