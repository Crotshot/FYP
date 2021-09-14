using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player_Controller : MonoBehaviour
{
    GameObject navTarget;
    NavMeshAgent agent;
    Inputs inputs;

    
    Animator animator;

    [SerializeField] AnimationMovement[] animations; //Booleans
    /*~ - Idle
    * 0 - Move
    * 1 - Attack
    * 2 - Ability1
    * 3 - Ability2
    * 4 - Ability3
    */

    [SerializeField] float agentSpeed = 4.1f, agentAngularSpeed = 270f;
    AnimationMovement currentAnimation;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        navTarget = FindObjectOfType<Camera_Follower>().transform.GetChild(0).gameObject;
        inputs = FindObjectOfType<Inputs>();
        animator = GetComponentInChildren<Animator>();
        agent.speed = agentSpeed;
        agent.angularSpeed = agentAngularSpeed;
    }

    private void Update()
    {
        Vector3 moveTarget = inputs.MovementInput();
        animator.SetBool(animations[0].animationName, false);
        if (moveTarget != Vector3.zero)
            animator.SetBool(animations[0].animationName, true);
        navTarget.transform.localPosition = moveTarget;
        agent.destination = navTarget.transform.position;

        AnimationControl();

        AnimatorClipInfo[] m_CurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0); //Layer 0 -> Gets active animations
        for (int i = 0; i < m_CurrentClipInfo.Length; i++)
        {
            Debug.Log(m_CurrentClipInfo[i].clip.name); //Only one avtive animation
        } 
    }

    private void AnimationControl()
    {
        animator.SetBool(animations[1].animationName, false);
        animator.SetBool(animations[2].animationName, false);
        animator.SetBool(animations[3].animationName, false);
        animator.SetBool(animations[4].animationName, false);

        if (inputs.GetAttackInput() > 0)
        {
            animator.SetBool(animations[1].animationName, true);
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