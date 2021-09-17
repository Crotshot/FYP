using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player_Controller : MonoBehaviour
{
    Transform navTarget;//, animatedNavTarget;
    NavMeshAgent agent;
    [SerializeField] float agentSpeed = 4.1f, agentAngularSpeed = 270f;

    Inputs inputs;
    bool locked = false;

    [SerializeField] AnimationMovement[] animations;
    Animator animator;
    AnimationClip currentClip;
    AnimatorStateInfo stateInfo;
    int animationIndex = -1, percentChangeIndex = -1;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        inputs = FindObjectOfType<Inputs>();

        navTarget = transform.GetChild(1).GetChild(1);
        //animatedNavTarget = transform.GetChild(2);
    }

    private void Start()
    {
        agent.speed = agentSpeed;
        agent.angularSpeed = agentAngularSpeed;

        AnimatorClipInfo[] currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        currentClip = currentClipInfo[0].clip;
    }

    private void Update()
    {
        if (!locked)
        {
            navTarget.localPosition = inputs.GetMovementInput();
            if (navTarget.localPosition != Vector3.zero)
                animator.SetBool("Moving", true);
            else
                animator.SetBool("Moving", false);
            Debug.Log("Banana");
            agent.SetDestination(navTarget.position); // Use SetDest so it updates each time its changed
        }

        AnimationControls();
        UpdateCurrentClip();
        AnimationRestrictions();
    }

    //Get inputs and set animator bools accordingly
    private void AnimationControls()
    {
        for (int i = 0; i < animations.Length; i++)
        {
            if (animations[i].animName.Equals("") || animations[i].animName.Contains("Recovery"))
            {
                continue;
            }
            if (inputs.GetAnyInput(animations[i].animName) > 0)
            {
                animator.SetBool(animations[i].animName, true);
            }
            else
            {
                animator.SetBool(animations[i].animName, false);
            }
        }
    }

    //Gets the currently active animation clip and sets the current animations[] index
    private void UpdateCurrentClip()
    {
        AnimatorClipInfo[] currentClipInfo = animator.GetCurrentAnimatorClipInfo(0); //Layer 0 -> Gets active animations, I only have one that is grabbed
        if (currentClipInfo.Length > 0)
        {
            if (!currentClipInfo[0].clip.name.Equals(currentClip.name))
            {
                currentClip = currentClipInfo[0].clip; //Get the current clip

                animationIndex = -1;
                percentChangeIndex = -1;
                locked = false;
                for(int i = 0; i < animations.Length; i++)
                {
                    if (currentClip.name.Contains(animations[i].animName))
                    {
                        if(animations[i].percentChange.Length > 0)
                        {
                            percentChangeIndex = 0;
                            animationIndex = i;
                        }
                        break;
                    }
                }

                agent.speed = agentSpeed;
                agent.angularSpeed = agentAngularSpeed;
            }
        }

        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    }


    //Change players control over conqueror while using animations
    private void AnimationRestrictions()
    {
        if (animationIndex == -1)
            return;
        else if (animations[animationIndex].lockMoveRot)
            locked = true;

        if (percentChangeIndex == -1)
            return;

        // If percentage of completion is               >       index percent change 
        if((stateInfo.normalizedTime / stateInfo.length) > animations[animationIndex].percentChange[percentChangeIndex])
        {
            percentChangeIndex++; //Increment
            if(percentChangeIndex == animations[animationIndex].percentChange.Length)
            {
                percentChangeIndex = -1;
                return;
            }
            //Update speed when pChangeIndex is changed
            if (animations[animationIndex].agentSpeed[percentChangeIndex] > -1)//Do not change agent.speed if it is -1
            {
                agent.speed = animations[animationIndex].agentSpeed[percentChangeIndex];
            }
        }
        // Update position each call as the avater might reach the point if the times between are too long and stand still
        if (animations[animationIndex].direction[percentChangeIndex].y > -(Vector3.one).y) //Y value should always be zero so if it is less than 0 we ignore the direction
        {
            //animatedNavTarget.localPosition = (transform.position + animations[animationIndex].direction[percentChangeIndex]);
            //agent.SetDestination(animatedNavTarget.position);
            navTarget.position = transform.position + transform.TransformDirection(animations[animationIndex].direction[percentChangeIndex].x, 0, animations[animationIndex].direction[percentChangeIndex].z);
            agent.SetDestination(navTarget.position);
        }
    }
}


/*
bool animationIsInArray = false;
for (int i = 0; i < animations.Length; i++)
{
    if (currentClip.name.Contains(animations[i].animName)) //Check that were looking at the right animation
    {
        animationIsInArray = true;
        animationIndex = i;
        if (animations[i].lockMoveRot)
        {
            locked = true;
            agent.angularSpeed = 0;
        }
        else
        {
            agent.speed = agentSpeed;
            agent.angularSpeed = agentAngularSpeed;
            locked = false;
        }


        for (int j = 0; j < animations[i].percentChange.Length; j++)
        {
            if (animations[i].percentChange[j] >= (stateInfo.normalizedTime / stateInfo.length)) //Is the time of the animationMovment index > the % completion of the animation
            {
                if (animations[i].agentSpeed[j] > -1)//Do not change agent.speed if it is -1
                {
                    agent.speed = animations[i].agentSpeed[j];
                }
                if (animations[i].direction[j].y > -(Vector3.one).y) //Y value should always be zero so if it is less than 0 we ignore it
                {
                    navTarget.position = (transform.position + animations[i].direction[j]);
                    agent.SetDestination(navTarget.position);
                }
            }
        }

    }
}
if (!animationIsInArray)
{
    agent.speed = agentSpeed;
    agent.angularSpeed = agentAngularSpeed;
    locked = false;
}
*/
