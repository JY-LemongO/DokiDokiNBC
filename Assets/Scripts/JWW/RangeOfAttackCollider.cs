using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class RangeOfAttackCollider : MonoBehaviour //공격 가능 범위 스크립트
{
    private EnemyController enemtController;
    void Start()
    {
        enemtController = transform.GetComponentInParent<EnemyController>();//자신의 부모객체로부터 
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != enemtController.targetTag) //플레이어가 아니라면
            return;

        if (!enemtController.isAttacking)//공격중이 아닐때만
        {
            enemtController.isContect = true;
            enemtController.isAttacking = true;
        }
        //enemy1.ApplyMovement(Vector2.zero);
    }
    private void OnTriggerExit2D(Collider2D collision) //충돌이 끝나면
    {
        enemtController.isContect = false;
    }
}
