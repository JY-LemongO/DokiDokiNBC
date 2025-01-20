using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Processors;

public class BossMonster : MonoBehaviour
{
    private static readonly int die = Animator.StringToHash("Die"); //파라미터를 해시로 변환

    public GameObject bulletPrefab;//불릿
    public string bulletPrefabPath = "Monsters/MonsterBullet";
    public Transform firePoint;//발사 트랜스폼
    private Transform target;//플레이어의 트랜스폼
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private HealthSystem _healthSystem;
    private Vector2 moveDirection;

    public AudioClip deathClip;
    public AudioClip attackClip;

    public float moveSpeed = 5f;
    public LayerMask levelLayerMask;

    public float attackCooldown = 2f;//공격 쿨다운
    private float nextAttackTime = 0f;//다음 공격 시간

    public float moveCooldown = 4f;//이동 쿨다운
    private float nextMoveTime = 0f;//다음 이동 시간
    private float bulletSpeed = 5f;

    [SerializeField]
    private float healthPercentage = 100f;//보스 퍼센테이지
    private bool isDead = false;

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();
        _animator = GetComponentInChildren<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        moveDirection = transform.right;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        nextAttackTime = Time.time;
        nextMoveTime = Time.time;
        target = GameObject.FindWithTag("Player").transform;
    }
    private void Start()
    {
        Debug.Log($"{_healthSystem.CurrentHealth}");
        _healthSystem.OnDeath += Dead;
    }
    void Update()
    {
        healthPercentage = (_healthSystem.CurrentHealth / _healthSystem.MaxHealth) * 100f;
        if (!isDead)
        {
            FlipSprite();
            RotateFirePoint();
               
            if (Time.time >= nextMoveTime)
            {
                moveDirection = new Vector2(Random.Range(0,360),Random.Range(0,360)).normalized;
                _rigidbody2D.velocity = moveDirection * moveSpeed;
                nextMoveTime = Time.time + moveCooldown;
            }

            transform.right = moveDirection;
            _rigidbody2D.velocity = moveDirection * moveSpeed;

            if (Time.time >= nextAttackTime)
            {
                if (healthPercentage >= 70f)
                {
                    ExecuteSingleAttack();
                }
                else if (healthPercentage >= 40f && healthPercentage < 70f)
                {
                    attackCooldown = 3f;
                    ExecuteDoubleAttack();
                }
                else if (healthPercentage >= 20f && healthPercentage < 40f)
                {
                    attackCooldown = 5f;
                    ExecuteTripleAttack();
                }
                else if (healthPercentage > 0f && healthPercentage < 20f)
                {
                    attackCooldown = 7f;
                    ExecuteQuadrupleAttack();
                }
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    private void ExecuteQuadrupleAttack()
    {
        ExecuteRandomAttack(3,4);
        SoundManager.Instance.PlayClip(attackClip);
        Invoke("ExecuteTripleAttack",attackCooldown/6);
    }

    private void ExecuteTripleAttack()
    {
        ExecuteRandomAttack(4, 6);
        SoundManager.Instance.PlayClip(attackClip);
        Invoke("ExecuteDoubleAttack", attackCooldown/4);
    }

    private void ExecuteDoubleAttack()
    {
        ExecuteRandomAttack(3, 4);
        SoundManager.Instance.PlayClip(attackClip);
        Invoke("ExecuteSingleAttack", attackCooldown/5);
    }

    private void ExecuteSingleAttack()
    {
        ExecuteRandomAttack(1, 3);//1 ~ 2
        SoundManager.Instance.PlayClip(attackClip);
    }

    void ExecuteRandomAttack(int min,int max)//랜덤 공격
    {
        min = Mathf.Clamp(min,1, 6);
        max = Mathf.Clamp(max,1, 6);
        int attackPattern = Random.Range(min, max); // 1부터 5까지의 무작위 패턴 선택

        switch (attackPattern)
        {
            case 1:
                FireStraight();
                break;
            case 2:
                FireSpread();
                break;
            case 3:
                FireCircle();
                break;
            case 4:
                FireDiverge();
                break;
            case 5:
                FireRandomDirection();
                break;
            default:
                Debug.Log("잘못된 공격 패턴입니다.");
                break;
        }
    }

    void FireStraight()//직선으로 하나 공격
    {
        //GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        GameObject bullet = Managers.RM.Instantiate(bulletPrefabPath);
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        MonsterBullet newbullt = bullet.GetComponent<MonsterBullet>();
        newbullt.speed = bulletSpeed;
    }

    void FireSpread()//바라보는 방향으로 5개 뿌리기
    {
        for (int i = -2; i <= 2; i++)
        {
            //GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(0f, 0f, i * 15f));
            GameObject bullet = Managers.RM.Instantiate(bulletPrefabPath);
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation * Quaternion.Euler(0f, 0f, i * 15f);
            MonsterBullet newbullt = bullet.GetComponent<MonsterBullet>();
            newbullt.speed = bulletSpeed;
        }
    }

    void FireCircle()//자신을 중심으로 원형으로 뻗어나가는 공격
    {
        for (int i = 0; i < 360; i += 20)
        {
            //GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0f, 0f, i));
            GameObject bullet = Managers.RM.Instantiate(bulletPrefabPath);
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, i);
            MonsterBullet newbullt = bullet.GetComponent<MonsterBullet>();
            newbullt.speed = bulletSpeed;
        }
    }
    void FireDiverge()//두개를 던지고 잠시후 원으로 퍼지는 공격
    {
        for (int i = -1; i <= 0; i++)
        {
            //GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(0f, 0f, i * 30f + 15f));
            GameObject bullet = Managers.RM.Instantiate(bulletPrefabPath);
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation * Quaternion.Euler(0f, 0f, i * 30f + 15f);
            MonsterBullet newbullt = bullet.GetComponent<MonsterBullet>();
            newbullt.speed = bulletSpeed;
            newbullt.SpreadBulletInOnePoint();
        }
    }

    void FireRandomDirection()//5개 공격을 뿌리고 적을 추적하는 공격
    {
        for (int i = 0; i < 5; i++)
        {
            float randomAngle = Random.Range(0f, 360f);
            //GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0f, 0f, randomAngle));
            GameObject bullet = Managers.RM.Instantiate(bulletPrefabPath);
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, randomAngle);
            MonsterBullet newbullt = bullet.GetComponent<MonsterBullet>();
            newbullt.speed = bulletSpeed;
            newbullt.TrackingBullet();
        }
    }
    public void RotateFirePoint()//발사 위치와 발사 회전을 플레이어쪽으로 수정.
    {
        firePoint.localPosition = (target.transform.position- transform.position).normalized; //보스에서 플레이어를 향한 단위 벡터만큼 떨어진 곳
        firePoint.right = (target.transform.position - transform.position).normalized;//발사 위치의 오른쪽을 플레이어 방향으로 설정
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 레벨과 충돌한 경우
        if (collision.gameObject.layer == LayerMask.NameToLayer("Level") || collision.gameObject.tag == ("Player") || collision.gameObject.tag == ("Monster"))
        {
            // 벽의 법선 벡터를 가져옴
            Vector3 wallNormal = collision.contacts[0].normal;

            // 입사각과 반사각이 동일하도록 보스의 이동 방향을 조정
            Vector3 reflectedDirection = Vector2.Reflect(moveDirection, wallNormal);
            moveDirection = reflectedDirection.normalized;
        }
    }
    public void FlipSprite()
    {
        if (transform.right.x < 0)
            _spriteRenderer.flipY = true;
        else
            _spriteRenderer.flipY = false;
    }
    private void Dead()
    {
        _rigidbody2D.velocity = Vector2.zero;
        isDead = true;
        foreach (Collider2D component in transform.GetComponentsInChildren<Collider2D>())
        {
            component.enabled = false;
        }
        _animator.SetTrigger(die);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            
        }
    }
}