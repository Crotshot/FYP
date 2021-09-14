using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player_Controller : MonoBehaviour
{
    GameObject navTarget;
    NavMeshAgent agent;
    Inputs inputs;

    [SerializeField] AnimationMovement[] animations; //Booleans
    /*~ - Idle
    * 0 - Move
    * 1 - Attack
    * 2 - Ability1
    * 3 - Ability2
    * 4 - Ability3
    */

    [SerializeField] float agentSpeed = 4.1f, agentAngularSpeed = 270f;
    bool locked = false;

    Animator animator;
    AnimationClip currentClip;
    AnimatorStateInfo stateInfo;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        navTarget = FindObjectOfType<Camera_Follower>().transform.GetChild(0).gameObject;
        inputs = FindObjectOfType<Inputs>();
        animator = GetComponentInChildren<Animator>();
        agent.speed = agentSpeed;
        agent.angularSpeed = agentAngularSpeed;

        AnimatorClipInfo[] currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        currentClip = currentClipInfo[0].clip;
    }

    private void Update()
    {
        if (!locked)
        {
            Vector3 moveTarget = inputs.MovementInput();
            animator.SetBool(animations[0].animationName, false);
            if (moveTarget != Vector3.zero)
                animator.SetBool(animations[0].animationName, true);
            navTarget.transform.localPosition = moveTarget;
            agent.destination = navTarget.transform.position;
        }

        AnimationControls();
        AnimatorClipInfo[] currentClipInfo = animator.GetCurrentAnimatorClipInfo(0); //Layer 0 -> Gets active animations, I only have one that is grabbed
        for (int i = 0; i < currentClipInfo.Length; i++)
        {
            if (!currentClipInfo[i].clip.name.Equals(currentClip.name)){ 
                currentClip = currentClipInfo[i].clip;//Get the current clip
            }
        }

        stateInfo = animator.GetCurrentAnimatorStateInfo(0); //Get the state of the animator, do I need to assign multiple times?
        AnimationRestrictions();
    }

    //Change players control over conqueror while using animations
    private void AnimationRestrictions()
    {
        for(int i = 0; i < animations.Length; i++)
        {
            if (animations[i].animationName.Equals(currentClip.name)) //Check that were looking at the righrt animation
            {
                if (animations[i].lockMoveRot)
                {
                    locked = true;
                    agentAngularSpeed = 0;
                }
                else
                {
                    agent.speed = agentSpeed;
                    agent.angularSpeed = agentAngularSpeed;
                    locked = false;
                }
                for (int j = 0; j < animations[i].percentChange.Length; j++)
                {
                    if (animations[i].percentChange[j] >= (stateInfo.normalizedTime/stateInfo.length)) //Is the time of the animationMovment index > the % completion of the animation
                    {
                        if(animations[i].agentSpeed[j] > -1)//Do not change agent.speed if it is -1
                        {
                            agent.speed = animations[i].agentSpeed[j];
                        }
                        if (animations[i].direction[j].y > -(Vector3.one).y) //Y value should always be zero so if it is less than 0 we ignore it
                        {
                            agent.destination = animations[i].direction[j];
                        }
                    }
                }
            }
        }
    }

    private void AnimationControls()
    {
        animator.SetBool(animations[1].animationName, false);
        animator.SetBool(animations[2].animationName, false);
        animator.SetBool(animations[3].animationName, false);
        animator.SetBool(animations[4].animationName, false);
        animator.SetBool(animations[5].animationName, false);
        animator.SetBool(animations[6].animationName, false);

        if (inputs.GetAttackInput() > 0)
        {
            animator.SetBool(animations[1].animationName, true);
            animator.SetBool(animations[5].animationName, true);
            animator.SetBool(animations[6].animationName, true);
        }
        if (inputs.GetAbility1Input() > 0)
        {
            animator.SetBool(animations[2].animationName, true);
        }
        if (inputs.GetAbility2Input() > 0)
        {
            animator.SetBool(animations[3].animationName, true);
        }
        if (inputs.GetAbility3Input() > 0)
        {
            animator.SetBool(animations[4].animationName, true);
        }
    }
}