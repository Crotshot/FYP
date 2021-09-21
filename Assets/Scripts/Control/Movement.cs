using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
//Moves an object to its nav destination and updates the positon of its 2D sprite counter part

    Transform spriteCounterPart;
    NavMeshAgent agent;
    [SerializeField] Transform navTarget;
    public void Setup()
    {
        agent = GetComponent<NavMeshAgent>();

        spriteCounterPart = transform.parent;
        spriteCounterPart.transform.position = Vector3.zero;
        transform.parent = null;
    }

    public void SetSpeeds(float agentSpeed, float agentAngularSpeed)
    {
        agent.speed = agentSpeed;
        agent.angularSpeed = agentAngularSpeed;
    }

    private void Update()
    {
        agent.SetDestination(navTarget.position);
        TranslateTo2D();
    }

    public void SetNavTarget(Vector3 coord, bool local)
    {
        if (local)
        {
            navTarget.localPosition = coord;
        }
        else
        {
            navTarget.position = coord;
        }
    }

    private void TranslateTo2D()
    {
        spriteCounterPart.transform.position = new Vector3(transform.position.x, transform.position.z, 0.01f);
        Vector3 rot = transform.rotation.eulerAngles;
        spriteCounterPart.rotation = Quaternion.Euler(0, 0, -rot.y);
    }

    public Transform GetNavTarget()
    {
        return navTarget;
    }
}