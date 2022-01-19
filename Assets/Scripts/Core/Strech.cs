using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers = Crotty.Helpers.StaticHelpers;

public class Strech : MonoBehaviour
{
    [SerializeField] Transform pointA, pointB;
    [SerializeField] float objectLength;

    private void Update() {
        transform.position = transform.position = pointA.position;
        transform.LookAt(pointB);
        transform.localScale = new Vector3(1,1, Helpers.Vector3Distance(pointA.position, pointB.position) / objectLength);
    }
}
