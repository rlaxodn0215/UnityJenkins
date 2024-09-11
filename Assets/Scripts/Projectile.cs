using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Movement2D movement2D;
    private Transform target;
    private float damage;

    public void Setup(Transform target, float damage)
    {
        movement2D = GetComponent<Movement2D>();
        // 타워가 설정해준 타겟
        this.target = target;
        // 타워가 설정해준 공격력
        this.damage = damage;
    }

    private void Update()
    {
        // 타겟이 존재하면
        if(target != null)
        {
            // 발사체 타겟으로 이동
            Vector3 direction = (target.position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        // 여러 이유로 타겟이 사라지면
        else
        {
            // 발사체 삭제
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 적이 아닌 대상과 부딪히면
        if (!collision.CompareTag("Enemy")) return;
        // 현재 타겟인 적이 아닐 때
        if (collision.transform != target) return;

        // 적 사망 함수 호출
        //collision.GetComponent<Enemy>().OnDie();
        // 적 제력 감소
        collision.GetComponent<EnemyHP>().TakeDamage(damage);
        // 발사체 삭제
        Destroy(gameObject);
    }
}
