using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colors : MonoBehaviour
{
    [SerializeField] public Color redConqCol, blueConqCol, redMinionCol, blueMinionCol, red, blu, black;
    [SerializeField] public Material team1, team2;

    public Material GetTeamMaterial(int team) {
        if(team == 1) {
            return team1;
        }
        return team2;
    }

    public Color GetTeamColor(int team) {
        if (team == 1) {
            return red;
        }
        if (team == 2) {
            return blu;
        }
        return black;
    }
}
