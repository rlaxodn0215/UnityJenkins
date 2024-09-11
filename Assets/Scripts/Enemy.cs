using System.Collections;
using UnityEngine;

public enum EnemyDestroyType
{
    Kill,
    Arrive
}

public class Enemy : MonoBehaviour
{
    // 이동 경로 개수
    private int wayPointCount;
    // 이동 경로 정보
    private Transform[] wayPoints;
    // 현재 목표지점 인덱스
    private int currentIndex = 0;
    //오브젝트 이동 제어
    private Movement2D movement2D;
    // 적의 삭제를 본인이 하지 않고 EnemySpawner에서 삭제
    private EnemySpawner enemySpawner;
    // 적 사망시 획득 가능한 골드
    [SerializeField]
    private int gold = 10;

    public void Setup(EnemySpawner enemySpawner, Transform[] wayPoints)
    {
        movement2D = GetComponent<Movement2D>();
        this.enemySpawner = enemySpawner;

        // 적 이동 경로 wayPoints 정보 살정
        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;

        // 적의 위치를 첫번째 wayPoint 위치로 설정
        transform.position = wayPoints[currentIndex].position;

        // 적 이동 코루틴 시작
        StartCoroutine("OnMove");
    }

    private IEnumerator OnMove()
    {
        // 다음 이동 방향 설정
        NextMoveTo();

        while (true)
        {
            // 적 오브젝트 회전
            //transform.Rotate(Vector3.forward * 10.0f);

            // 적의 현재위치와 목표위치의 거리가 0.02f * movement2D.MoveSpeed보다 작을 때 조건문 발생
            // movement2D.MoveSpeed 곱하는 이유 : 속도가 빠르면 한 프레임에 0.02 보다 크게 움직이기 때문에 조건문 탈주 위험
            if(Vector3.Distance(transform.position, wayPoints[currentIndex].position) < 0.02f * movement2D.MoveSpeed)
            {
                // 다음 이동 방향 설정
                NextMoveTo();
            }

            yield return null;
        }
    }

    private void NextMoveTo()
    {
        // 아직 이동할 wayPoints가 남아 있다면
        if(currentIndex < wayPointCount - 1)
        {
            // 적의 위치를 정확하게 목표 위치로 설정
            transform.position = wayPoints[currentIndex].position;
            // 이동 방향 설정 -> 다음 목표지점
            currentIndex++;
            Vector3 direction = (wayPoints[currentIndex].position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        // 현재 위치가 마지막 wyaPoints이면
        else
        {
            gold = 0;
            // 적 오브젝트 삭제
            OnDie(EnemyDestroyType.Arrive);
        }
    }

    public void OnDie(EnemyDestroyType type)
    {
        // EnemySpawner에서 삭제
        enemySpawner.DestoryEnemy(type ,this, gold);
    }

}
