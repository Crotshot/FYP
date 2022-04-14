using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Mirror;

public class ControlPoint : NetworkBehaviour {
    [SerializeField] Image captureFillImage;
    [SerializeField] Color fillColor;
    [SerializeField] private float chargesToCapture, currentCharges, basePointMultiplier = 0;
    [SerializeField] private int assignedMinions_1, assignedMinions_2, startingMaxMinions = 3, mtsMinionIncrease = 7, currentTeam = 0;
    [SerializeField] bool basePoint;
    List<Transform> trackedChars;
    int currentMaxMinions;
    bool setUp;
    Colors cols;

    private enum CaptureState { Neutral, Captured } //Neutral has leaning for teams using current team;
    [SerializeField] CaptureState capState = CaptureState.Neutral;//[SerializeField] FOR DEBUGGING

    public UnityEvent captured, neutralised;

    private void Start() {
        trackedChars = new List<Transform>();

        if (isServer) {
            if (captured == null) {
                captured = new UnityEvent();
            }
            if (neutralised == null) {
                neutralised = new UnityEvent();
            }
        }
    }

    List<Team> teams = new List<Team>(); //Used for Rpcing the colour 
    public void Setup() {
        RpcSetup();
    }

    [ClientRpc]
    private void RpcSetup() {
        foreach (PlayerMinions pM in FindObjectsOfType<PlayerMinions>()) {
            if (!teams.Contains(pM.GetComponent<Team>())) {
                teams.Add(pM.GetComponent<Team>());
            }
        }
        setUp = true;
        cols = FindObjectOfType<Colors>();
        currentMaxMinions = startingMaxMinions;
    }

    float teamCharges;
    int teamLean;
    private void Update() {
        #region Capturing
        teamCharges = 0;
        teamLean = 0;
        for (int i = trackedChars.Count - 1; i > -1; i--) {
            if (trackedChars[i] == null) {
                trackedChars.Remove(trackedChars[i]);
                continue;
            }
            if (trackedChars[i].GetComponent<Team>().GetTeam() == teamLean) {
                if (trackedChars[i].tag.Equals("Player"))
                    teamCharges += 10f * Time.deltaTime;
                else
                    teamCharges += 1f * Time.deltaTime;
            }
            else {
                if (trackedChars[i].tag.Equals("Player"))
                    teamCharges -= 10f * Time.deltaTime;
                else
                    teamCharges -= 1f * Time.deltaTime;

                if (teamCharges <= 0) {
                    teamCharges *= -1;
                    teamLean = trackedChars[i].GetComponent<Team>().GetTeam();
                    fillColor = cols.GetTeamColor(teamLean);
                }
            }
        }

        if (teamCharges == 0)
            return;
        #endregion

        teamCharges *= currentTeam == teamLean ? 1 : -1;
        currentCharges += teamCharges;
        if (capState == CaptureState.Neutral) {    
            if (currentCharges >= chargesToCapture) {
                if (isServer)
                    RpcReflect(teamLean, chargesToCapture, 1);
            }
            else if (currentCharges <= 0) {
                currentCharges *= -1;
                if (isServer)
                    RpcReflect(teamLean, currentCharges, 0);
            }
        }
        else {//capState == CaptureState.Captured
            if (currentCharges >= chargesToCapture) {
                currentCharges = chargesToCapture;
            }
            else if (currentCharges <= 0) {
                currentCharges *= -1;
                if (isServer) {
                    neutralised?.Invoke();
                    RpcReflect(teamLean, currentCharges, 0);
                }
            }
        }
        captureFillImage.fillAmount = currentCharges / chargesToCapture;
    }

    [ClientRpc]
    private void RpcReflect(int team, float cCharges, int state) {
        currentCharges = cCharges;
        capState = (CaptureState)state;

        fillColor = cols.GetTeamColor(team);
        currentTeam = team;
        captureFillImage.color = fillColor;
        captureFillImage.fillAmount = currentCharges / chargesToCapture;

        if(isServer)
            captured?.Invoke();
    }

    private void OnTriggerEnter(Collider other) {
        if (!setUp)
            return;
        if (other.tag.Equals("minion") || other.tag.Equals("Player") && !trackedChars.Contains(other.transform)) {
            trackedChars.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!setUp)
            return;
        if (other.tag.Equals("minion") || other.tag.Equals("Player") && trackedChars.Contains(other.transform)) {
            trackedChars.Remove(other.transform);
        }
    }

    public int GetOwningTeam() {
        if (capState == CaptureState.Captured) {
            return currentTeam;
        }
        else {
            return 0;
        }
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

    public int GetCurrentMaxMinions() {
        return currentMaxMinions;
    }

    public void AddMinion(int team) {
        if (team == 1) {
            assignedMinions_1++;
        }
        else if (team == 2) {
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

/*
    public void Start() {
        if (isServer)
            captured = new UnityEvent<int>();
    }

    private void FixedUpdate() {
        if (isServer) {

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




    public bool GetBasePoint() {
        return basePoint;
    }



    public void SetMaxMinions(int newMax) {
        maxAssignedMinions = newMax;
    }
*/
