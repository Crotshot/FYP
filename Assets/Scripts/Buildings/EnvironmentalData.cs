using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//REDUNDENT SCRIPT, DELETE LATER IF NO USE ARISES
public class EnvironmentalData : MonoBehaviour
{
    [SerializeField] Vector2[] buildingPositions, propPositions;

    public Vector2[] GetBuildingPositions() {
        return buildingPositions;
    }

    public Vector2[] GetPropPositions() {
        return propPositions;
    }
}