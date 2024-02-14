using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Enemy1;

public class Enemy1 : EnemyController
{
    public enum MonsterNumber
    {
        Enmemy1,
        Enmemy3
    }
    public MonsterNumber monsterNumber;
    private Rigidbody2D _rigidbody;
    private Vector2 _movementDirection = Vector2.zero;
    public CharacterStatsHandler characterStatsHandler;
    private Collider2D _collider;
    public bool isContect = false;
    protected override void Awake()
    {
        base.Awake();
        monsterNumber = enemySO.monsterNumber;
        characterStatsHandler = GetComponent<CharacterStatsHandler>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    protected override void Start()
    {
        base.Start();
        OnMoveEvent += Move;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Vector2 direction = Vector2.zero;
        if (!isContect && !isDead)//접촉하지 않았을 때 공격중이 아닐 때
        {
            if(!isAttacking)
                direction = GetDirection();
        }
        ApplyMovement(_movementDirection);
        CallMoveEvent(direction);
        Rotate(direction);
    }
    private void Move(Vector2 direction)
    {
        _movementDirection = direction;
    }
    public void ApplyMovement(Vector2 direction)
    {
        direction = direction * characterStatsHandler.CurrentStates.speed;//몬스터의 스피드 만큼 속도 적용
        _rigidbody.velocity = direction;
    }   
    private void Rotate(Vector2 direction)
    {
        if (direction != Vector2.zero)//
        {
            int num = 1;
            if (monsterNumber == MonsterNumber.Enmemy1)
            {
                num = direction.x < 0 ? 1 : -1;
            }
            else if (monsterNumber == MonsterNumber.Enmemy3)
            {
                num = direction.x < 0 ? -1 : 1;
            }
            transform.localScale = new Vector3(num, 1, 1);
        }
    }
}
