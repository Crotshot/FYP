using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : MonoBehaviour
{
    Movement movement;
    [SerializeField] float strayDistance, agentSpeed, agentAngularSpeed;
    GameObject pointOfInterest;

    private void Awake()
    {
        movement = GetComponentInChildren<Movement>();
        movement.Setup();
        movement.SetSpeeds(agentSpeed, agentAngularSpeed);
    }

    private void Update()
    {
        
    }
}
