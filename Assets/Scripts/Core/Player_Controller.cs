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
            if (animations[i].animatorBool.Equals("") || animations[i].animName.Contains("Recovery"))
            {
                continue;
            }
            if (inputs.GetAnyInput(animations[i].animatorBool) > 0)
            {
                animator.SetBool(animations[i].animatorBool, true);
            }
            else
            {
                animator.SetBool(animations[i].animatorBool, false);
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
                    if (currentClip.name.Equals(animations[i].animName))
                    {
                        animationIndex = i;
                        if (animations[i].percentChange.Length > 0)
                        {
                            percentChangeIndex = 0;
                            agent.speed = animations[animationIndex].agentSpeed[percentChangeIndex];
                        }
                        break;
                    }
                }

                if (animationIndex == -1)
                {
                    agent.speed = agentSpeed;
                    agent.angularSpeed = agentAngularSpeed;
                }
            }
        }

        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    }


    //Change players control over conqueror while using animations
    private void AnimationRestrictions()
    {
        if (animationIndex == -1)
            return;
        else if (animations[animationIndex].lockMoveRot) //---------------------------------------------------------This entire function does not work
        {
            locked = true;
            agent.angularSpeed = 0;
        }

        if (percentChangeIndex == -1)
            return;

        // If percentage of completion is               >       index percent change
        Debug.Log((stateInfo.normalizedTime / stateInfo.length));
        if ((stateInfo.normalizedTime / stateInfo.length) > animations[animationIndex].percentChange[percentChangeIndex])
        {
            percentChangeIndex++; //Increment
            if(percentChangeIndex == animations[animationIndex].percentChange.Length)
            {
                percentChangeIndex = -1;
                return;
            }
            agent.speed = animations[animationIndex].agentSpeed[percentChangeIndex];
        }
        navTarget.position = transform.position + transform.TransformDirection(animations[animationIndex].direction[percentChangeIndex].x, 0, animations[animationIndex].direction[percentChangeIndex].z);
        agent.SetDestination(navTarget.position);
    }
}