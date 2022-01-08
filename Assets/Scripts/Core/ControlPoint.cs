using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ControlPoint : MonoBehaviour
{
    [SerializeField] private float chargesToCapture;
    [SerializeField] Image captureFillImage;
    [SerializeField] bool basePoint;
    //TEMP Serialized for testing//////////////////
    [SerializeField]private float charges;
    [SerializeField]int currentTeam = 0;
    ///////////////////////////////////////////////
    List<Transform> trackedTransforms = new List<Transform>();
    List<int> teams = new List<int>();
    Color color;
    private enum TeamState { Neutral, Captured}
    TeamState capState = TeamState.Neutral;

    public UnityEvent captured;

    private void Start() {
        if (captured == null)
            captured = new UnityEvent();
    }

    private void FixedUpdate() {
        float teamCharges = 0;
        int teamLean = 0;
        foreach (Transform form in trackedTransforms) {
            if (form == null) {
                trackedTransforms.Remove(form);
                continue;
            }

            if ( form.GetComponent<Team>().GetTeam() == teamLean) {
                if (form.tag.Equals("Player")) teamCharges += 10f * Time.deltaTime; else teamCharges += 1f * Time.deltaTime;
            }
            else {
                if (form.tag.Equals("Player")) teamCharges -= 10f * Time.deltaTime; else teamCharges -= 1f * Time.deltaTime;
                if(teamCharges <= 0) {
                    teamCharges *= -1;
                    teamLean = form.GetComponent<Team>().GetTeam();
                    color = form.GetComponent<Team>().GetTeamColor();
                }
            }
        }
        if (teamCharges == 0)
            return;
       Capturing(teamCharges, teamLean, color);
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
}