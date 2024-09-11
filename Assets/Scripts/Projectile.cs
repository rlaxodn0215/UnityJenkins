using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Movement2D movement2D;
    private Transform target;
    private float damage;

    public void Setup(Transform target, float damage)
    {
        movement2D = GetComponent<Movement2D>();
        // Ÿ���� �������� Ÿ��
        this.target = target;
        // Ÿ���� �������� ���ݷ�
        this.damage = damage;
    }

    private void Update()
    {
        // Ÿ���� �����ϸ�
        if(target != null)
        {
            // �߻�ü Ÿ������ �̵�
            Vector3 direction = (target.position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        // ���� ������ Ÿ���� �������
        else
        {
            // �߻�ü ����
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ���� �ƴ� ���� �ε�����
        if (!collision.CompareTag("Enemy")) return;
        // ���� Ÿ���� ���� �ƴ� ��
        if (collision.transform != target) return;

        // �� ��� �Լ� ȣ��
        //collision.GetComponent<Enemy>().OnDie();
        // �� ���� ����
        collision.GetComponent<EnemyHP>().TakeDamage(damage);
        // �߻�ü ����
        Destroy(gameObject);
    }
}
