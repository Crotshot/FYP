using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colors : MonoBehaviour
{
    [SerializeField] public Color enemyConqCol, redMinionCol, blueMinionCol;
    [SerializeField] public Material team1, team2;

    public Material GetTeamMaterial(int team) {
        if(team == 1) {
            return team1;
        }
        return team2;
    }
}
