using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Team : NetworkBehaviour
{
    [SerializeField] int team;
    [SerializeField] Color teamColor;

    public void AssignTeam() {
        team = Random.Range(-10000000,10000000);
    }

    public int GetTeam() {
        return team;
    }

    public Color GetTeamColor() {
        return teamColor;
    }
}