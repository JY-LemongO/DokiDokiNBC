using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : EnemyAnimation
{
    private static readonly int isWalking = Animator.StringToHash("IsWalking"); //파라미터를 해시로 변환
    private static readonly int isAttacking = Animator.StringToHash("Attack"); //파라미터를 해시로 변환
    private static readonly int isDead = Animator.StringToHash("Die"); //파라미터를 해시로 변환
    private static readonly int isHit = Animator.StringToHash("Hit"); //파라미터를 해시로 변환

    private HealthSystem healthSystem;
    protected override void Awake()
    {
        base.Awake();
        healthSystem = GetComponent<HealthSystem>();
    }
    private void Start()
    {
        enemyController.OnMoveEvent += Move; // 이동 구독
        enemyController.OnAttackEvent += Attack;// 공격 구독

        if (healthSystem != null)
        {
            healthSystem.OnDeath += Dead;
            healthSystem.OnDamage += Hit;
        }
    }

    private void Attack(EnemySO enemySO) //공격
    {
        animator.SetTrigger(isAttacking); // 공격 애니메이션 트리거 발동
    }

    private void Move(Vector2 direction) // 이동 
    {
        animator.SetBool(isWalking, direction.magnitude > 0.5f);
    }
    private void Hit()
    {
        animator.SetTrigger(isHit);
    }
    private void Dead()
    {
        enemyController.isDead = true;
        foreach (Collider2D component in transform.GetComponentsInChildren<Collider2D>())
        {
            component.enabled = false;
        }
        animator.SetTrigger(isDead);
        Destroy(gameObject,4f);
    }
}
