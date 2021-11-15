using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    [SerializeField] int team;

    public void AssignTeam() {
        team = Random.Range(-10000000,10000000);
    }

    public int GetTeam() {
        return team;
    }
}