using System.Collections;
using UnityEngine;

public enum EnemyDestroyType
{
    Kill,
    Arrive
}

public class Enemy : MonoBehaviour
{
    // �̵� ��� ����
    private int wayPointCount;
    // �̵� ��� ����
    private Transform[] wayPoints;
    // ���� ��ǥ���� �ε���
    private int currentIndex = 0;
    //������Ʈ �̵� ����
    private Movement2D movement2D;
    // ���� ������ ������ ���� �ʰ� EnemySpawner���� ����
    private EnemySpawner enemySpawner;
    // �� ����� ȹ�� ������ ���
    [SerializeField]
    private int gold = 10;

    public void Setup(EnemySpawner enemySpawner, Transform[] wayPoints)
    {
        movement2D = GetComponent<Movement2D>();
        this.enemySpawner = enemySpawner;

        // �� �̵� ��� wayPoints ���� ����
        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;

        // ���� ��ġ�� ù��° wayPoint ��ġ�� ����
        transform.position = wayPoints[currentIndex].position;

        // �� �̵� �ڷ�ƾ ����
        StartCoroutine("OnMove");
    }

    private IEnumerator OnMove()
    {
        // ���� �̵� ���� ����
        NextMoveTo();

        while (true)
        {
            // �� ������Ʈ ȸ��
            //transform.Rotate(Vector3.forward * 10.0f);

            // ���� ������ġ�� ��ǥ��ġ�� �Ÿ��� 0.02f * movement2D.MoveSpeed���� ���� �� ���ǹ� �߻�
            // movement2D.MoveSpeed ���ϴ� ���� : �ӵ��� ������ �� �����ӿ� 0.02 ���� ũ�� �����̱� ������ ���ǹ� Ż�� ����
            if(Vector3.Distance(transform.position, wayPoints[currentIndex].position) < 0.02f * movement2D.MoveSpeed)
            {
                // ���� �̵� ���� ����
                NextMoveTo();
            }

            yield return null;
        }
    }

    private void NextMoveTo()
    {
        // ���� �̵��� wayPoints�� ���� �ִٸ�
        if(currentIndex < wayPointCount - 1)
        {
            // ���� ��ġ�� ��Ȯ�ϰ� ��ǥ ��ġ�� ����
            transform.position = wayPoints[currentIndex].position;
            // �̵� ���� ���� -> ���� ��ǥ����
            currentIndex++;
            Vector3 direction = (wayPoints[currentIndex].position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        // ���� ��ġ�� ������ wyaPoints�̸�
        else
        {
            gold = 0;
            // �� ������Ʈ ����
            OnDie(EnemyDestroyType.Arrive);
        }
    }

    public void OnDie(EnemyDestroyType type)
    {
        // EnemySpawner���� ����
        enemySpawner.DestoryEnemy(type ,this, gold);
    }

}
