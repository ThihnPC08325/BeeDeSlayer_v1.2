﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int TriggerSpecialAttack = Animator.StringToHash("TriggerSpecialAttack");
    private static readonly int TriggerAttack = Animator.StringToHash("TriggerAttack");

    private enum State { Chase, Attack, SpecialAttack }
    private State _currentState;

    [Header("Target & Ranges")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float specialAttackRange = 1.5f;

    [Header("Attack Settings")]
    [SerializeField] private float normalAttackCooldown = 2f;
    [SerializeField] private float specialAttackCooldown = 5f;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float postSpecialAttackDelay = 1f;

    [Header("Damage Settings")]
    [SerializeField] private float normalAttackDamage = 10f;
    [SerializeField] private float normalPen = 0f;
    [SerializeField] private float specialAttackDamage = 25f;
    [SerializeField] private float specialPen = 5f;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private float normalAttackCooldownTimer = 0;
    [SerializeField] private float specialAttackCooldownTimer = 0;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool canMove = true;
    [SerializeField] private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (isAttacking || !canMove) return;

        normalAttackCooldownTimer -= Time.deltaTime;
        specialAttackCooldownTimer -= Time.deltaTime;

        switch (_currentState)
        {
            case State.Chase:
                ChaseState();
                break;
            case State.Attack:
                StartCoroutine(AttackState());
                break;
            case State.SpecialAttack:
                StartCoroutine(SpecialAttackState());
                break;
        }

        // Update animation
        animator.SetBool(IsMoving, agent.velocity.magnitude > 0.1f);
    }

    private void ChaseState()
    {
        if (!canMove) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Look at player
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        if (distanceToPlayer <= attackRange)
        {
            agent.ResetPath();
            animator.SetBool(IsMoving, false);

            if (distanceToPlayer <= specialAttackRange && specialAttackCooldownTimer <= 0f)
            {
                _currentState = State.SpecialAttack;
            }
            else if (normalAttackCooldownTimer <= 0f)
            {
                _currentState = State.Attack;
            }
        }
        else
        {
            agent.SetDestination(player.position);
            animator.SetBool(IsMoving, true);
        }
    }

    private IEnumerator AttackState()
    {
        isAttacking = true;
        canMove = false;
        agent.ResetPath();
        animator.SetBool(IsMoving, false);
        animator.SetTrigger(TriggerAttack);

        // Lấy độ dài của animation hiện tại
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        yield return new WaitForSeconds(attackDelay);

        // Gây sát thương
        GameEvents.TriggerPlayerHit(normalAttackDamage, normalPen);

        // Chờ animation kết thúc
        yield return new WaitForSeconds(animationLength - attackDelay);

        normalAttackCooldownTimer = normalAttackCooldown;
        isAttacking = false;
        canMove = true;
        _currentState = State.Chase;
    }

    private IEnumerator SpecialAttackState()
    {
        isAttacking = true;
        canMove = false;
        agent.ResetPath();
        animator.SetBool(IsMoving, false);
        animator.SetTrigger(TriggerSpecialAttack);

        // Lấy độ dài của animation hiện tại
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        yield return new WaitForSeconds(attackDelay);

        // Gây sát thương
        PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        if (playerHealth)
        {
            playerHealth.TakeDamage(specialAttackDamage, specialPen);
        }

        // Chờ animation kết thúc + thời gian delay sau special attack
        yield return new WaitForSeconds((animationLength - attackDelay) + postSpecialAttackDelay);

        specialAttackCooldownTimer = specialAttackCooldown;
        normalAttackCooldownTimer = normalAttackCooldown;
        isAttacking = false;
        canMove = true;
        _currentState = State.Chase;
    }

    // Debug helpers
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, specialAttackRange);
    }
}
