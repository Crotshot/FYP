using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Team : NetworkBehaviour
{
    [SerializeField][SyncVar] int team;
    [SerializeField][SyncVar] Color teamColor;
    [SerializeField] GameObject[] teamAccents;

    public int GetTeam() {
        return team;
    }

    public Color GetTeamColor() {
        return teamColor;
    }

    public void SetTeam(int t) {
        team = t;
    }

    public void SetTeamColor(float r, float g, float b, float a) { //Might need to be converted to 4 floats for network transport
        teamColor = new Color(r,g,b,a);

        Material mat = FindObjectOfType<Colors>().GetTeamMaterial(team);
        foreach (GameObject accent in teamAccents) {
            accent.GetComponent<Renderer>().material = mat;
        }
    }
}