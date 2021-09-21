using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //2D sprite mapped by the 3D object
    Inputs inputs;
    Movement movement;
    [SerializeField] float agentSpeed, agentAngularSpeed;

    private void Awake()
    {
        inputs = FindObjectOfType<Inputs>();
        movement = GetComponentInChildren<Movement>();
        movement.Setup();
        movement.SetSpeeds(agentSpeed, agentAngularSpeed);

        movement.transform.GetChild(0).GetComponent<Camera_Follower>().Setup();
    }

    private void Update()
    {
        movement.SetNavTarget(inputs.GetMovementInput(), true);
    }
}