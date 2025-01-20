using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private RegularEnemy enemyController;
    private Collider2D _collider2D;
    // Start is called before the first frame update
    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>() as RegularEnemy;
        _collider2D = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag != "Player")
            return;
        //collision.gameObject.GetComponent<HealthSystem>().ChangeHealth(-enemyController.enemySO.power);
        Managers.Player.GetDamaged();
    }
    public void PlayAttackSound()
    {
        SoundManager.Instance.PlayClip(enemyController?.attackClip);
    }
    public void PlayDeathSound()
    {
        SoundManager.Instance.PlayClip(enemyController?.deathClip);
    }
    public void EndOfAttack()
    {
        enemyController.isAttacking = false;
    }
}
