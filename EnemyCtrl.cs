using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.AI;

public class EnemyCtrl : MonoBehaviour
{
    public enum State 
    {
        Idle,
        Chase,
        Attack,
        Knockback,
        Guard
    }
    public bool attackState = false;
    public float attackSpeed = 5;
    public float attackRange = 3;
    public float attackStopDist = 2;
    public float chaseRange = 5;
    public float behaviorTime = 0;
    public float elapsedTime = 0;
    public float originalSpeed = 0;
    public float originAcceleration = 0;
    private float prevAttackTime;
    private State currentState = State.Idle;
    private bool isDead;
    public float moveSpeed;
    public PlayerCtrl player;
    Rigidbody rb;
    Animator animator;
    NavMeshAgent navMeshAgent;
    public Vector2 idleTime;

    private float guardTime = 3;
    private float guardElapsedTime = 0f;

    public void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerCtrl>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (attackState)
        {
            float time = Time.time - prevAttackTime;
            if (time >= attackSpeed)
            {
                attackState = false;
            }
        }
        if (isDead)
        {
            enabled = false;
            return;
        }
        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Knockback:
                Knockback();
                break;
            case State.Guard:
                Guard();
                break;
        }
    }
    public void CrossFadeInFixedTime(string name, float fixedTransitionDuration)
    {
        animator?.CrossFadeInFixedTime(name, fixedTransitionDuration);
    }
    public bool IsTag(string tag)
    {
        AnimatorStateInfo current = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo next = animator.GetNextAnimatorStateInfo(0);
        if (!animator.IsInTransition(0) && current.IsTag(tag))
            return true;
        else if (animator.IsInTransition(0) && next.IsTag(tag))
            return true;

        return false;
    }
    private void Knockback()
    {
        
    }

    private void Idle()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= chaseRange)
            {
                SetState(State.Chase);
            }
        }
    }
    public void Attack()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (player == null || player.isDead)
        {
            SetState(State.Idle);
            return;
        }
        if (distance > attackRange)
        {
            SetState(State.Chase);
            return;
        }
        if (attackState == false)
        {
            LookAt(player.transform.position);
            prevAttackTime = Time.time;
            attackState = true;
            animator.SetTrigger("Attack");
        }
    }
    public virtual void Chase()
    {
        if (player == null || player.isDead)
        {
            SetState(State.Idle);
            return;
        }
        if (IsTag("Knockback"))
            return;
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= attackRange)
        {
            SetState(State.Attack);
            return;
        }
        if(distance <= chaseRange)
        {
            Vector3 moveDirection = (player.transform.position - transform.position).normalized;
            navMeshAgent.SetDestination(player.transform.position);
        }
        else
        {
            SetState(State.Idle);
        }
    }
    public void Guard()
    {
        guardElapsedTime += Time.deltaTime;
        if (guardElapsedTime > guardTime)
        {
            SetState(State.Attack);
            guardElapsedTime = 0;
        }
    }    
    public void LookAt(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }
    public void SetState(State next)
    {
        switch (next)
        {
            case State.Idle:
                elapsedTime = 0;
                behaviorTime = UnityEngine.Random.Range(idleTime.x, idleTime.y);
                navMeshAgent.destination = transform.position;
                navMeshAgent.speed = originalSpeed;
                navMeshAgent.acceleration = originAcceleration;
                navMeshAgent.isStopped = true;
                animator.SetBool("Walk", false);
                break;
            case State.Attack:
                navMeshAgent.isStopped = true;
                animator.SetBool("Walk", false);
                break;
            case State.Chase:
                navMeshAgent.isStopped = false;
                animator.SetBool("Walk", true);
                break;
            case State.Knockback:
                animator.ResetTrigger("Attack");
                break;
            case State.Guard:
                navMeshAgent.isStopped = true;
                animator.SetBool("Guard", true);
                break;
        }
        currentState = next;
    }
}
