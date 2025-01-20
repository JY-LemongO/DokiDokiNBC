using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.XR.TrackedPoseDriver;

public class MonsterBullet : MonoBehaviour
{
    private Transform player; // 플레이어의 위치를 추적하기 위한 변수
    public float speed = 5f;// 총알 속도

    private float elapsedTime = 0f; // 경과 시간

    [Header("Tracking Bullet Info")]
    public float trackingTime = 3f; // 플레이어 추적 시간

    [Header("SpreadBulletInOnePoint Bullet Info")]
    public float keepGoingTime = 1f;

    public LayerMask targetMask;
    private Rigidbody2D _rigidbody;
    public string bulletPrefabPath = "Monsters/MonsterBullet";
    public float destroyTime = 3.5f;
    private float lifeTime = 0f;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        lifeTime = 0f;
        StartCoroutine(BulletDestroy(destroyTime));
    }
    IEnumerator BulletDestroy(float timeLimit)
    {
        while (lifeTime < timeLimit)
        {
            lifeTime += Time.deltaTime;
            yield return null;
        }
        if(gameObject != null)
            Managers.RM.Destroy(gameObject);//5초후 삭제
        yield return null;
    }
    void Update()
    {
        _rigidbody.velocity = transform.right * speed;
    }
    IEnumerator TrackingBulletCoroutine()//추적 불릿 코루틴
    {
        elapsedTime = 0f;
        while (true)
        {
            if (elapsedTime < trackingTime)
            {
                Vector2 direction = (Vector2)player.position - _rigidbody.position;
                direction.Normalize();
                float rotateAmount = Vector3.Cross(direction, transform.right).z;
                _rigidbody.angularVelocity = -rotateAmount * 200f; // 총알을 플레이어 방향으로 회전
                _rigidbody.velocity = transform.right * speed; // 총알을 플레이어 방향으로 이동
                yield return null;
                elapsedTime += Time.deltaTime;
            }
            else
            {
                _rigidbody.angularVelocity = 0f; // 추적 시간이 종료되면 회전 중지
                yield break;
            }
        }
    }
    IEnumerator SpreadBulletInOnePointCoroutine()
    {
        // 첫 번째 초 동안 총알들이 한 방향으로 직진
        elapsedTime = 0f;
        Vector3 initialDirection = transform.right; // 총알의 초기 방향

        while (elapsedTime < keepGoingTime)
        {
            //transform.Translate(initialDirection * speed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 첫 번째 초 이후, 총알들이 중심에서 모든 방향으로 퍼져나감
        Vector3 centerPoint = transform.position;

        for (int i = 0; i < 10; i++)
        {
            Vector3 spreadDirection = Quaternion.Euler(0f, 0f, i * 36f) * initialDirection; // 360도를 10등분하여 각도 계산
            //GameObject bullet = Instantiate(gameObject, centerPoint, Quaternion.identity); // 새로운 총알 생성
            GameObject bullet = Managers.RM.Instantiate(bulletPrefabPath);
            bullet.transform.position = centerPoint;
            bullet.transform.rotation = Quaternion.identity;
            MonsterBullet newBullet = bullet.GetComponent<MonsterBullet>();
            newBullet.transform.right = spreadDirection;
            yield return null;
        }

        // 모든 총알이 생성된 후, 원래 총알은 제거
        Managers.RM.Destroy(gameObject);
    }
    public void SpreadBulletInOnePoint()//탄 퍼짐 코루틴 호출
    {
        StartCoroutine(SpreadBulletInOnePointCoroutine());
    }
    public void TrackingBullet()//추적 불릿 코루틴 호출
    {
        StartCoroutine(TrackingBulletCoroutine());
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Level 레이어와 충돌할 경우 제거
        if (other.gameObject.layer == LayerMask.NameToLayer("Level"))
        {
            Managers.RM.Destroy(gameObject);
        }
        if (other.CompareTag("Player"))
        {
            Managers.Player.GetDamaged();
            Managers.RM.Destroy(gameObject);
        }
    }
}