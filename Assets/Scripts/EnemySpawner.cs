using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // 적 체력을 나타내는 Slider UI 프리팹
    [SerializeField]
    private GameObject enemyHPSliderPrefab;
    // UI를 표현하는 Canvas 오브젝트의 Transform
    [SerializeField]
    private Transform canvasTransform;
    // 현재 스테이지 이동 경로
    [SerializeField]
    private Transform[] wayPoints;
    // 플레이어의 체력 컴포넌트
    [SerializeField]
    private PlayerHP playerHP;
    // 플레이어의 골드 컴포넌트
    [SerializeField]
    private PlayerGold playerGold;
    // 현재 웨이브 종류
    private Wave currentWave;
    // 현재 웨이브에 남아있는 적 숫자
    private int currentEnemyCount;
    // 현재 맵에 있는 모든 적의 정보
    private List<Enemy> enemyList;

    // 적의 생성과 삭제는 EnemySpawner에서 하기 때문에 Set은 필요 없다
    public List<Enemy> EnemyList { get { return enemyList; } }
    // 현재 웨이브에 남아있는 적, 최대 적 숫자
    public int CurrentEnemyCount { get { return currentEnemyCount; } }
    public int MaxEnemyCount { get { return currentWave.maxEnemyCount; } }

    private void Awake()
    {
        // 적 리스트 메모리 할당
        enemyList = new List<Enemy>();
    }

    public void StartWave(Wave wave)
    {
        // 웨이브 정보 저장
        currentWave = wave;

        currentEnemyCount = currentWave.maxEnemyCount;

        // 현재 웨이브 시작
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy()
    {
        // 현재 웨이브에서 생성한 적 숫자
        int spawnEnemyCount = 0;

        while(spawnEnemyCount < currentWave.maxEnemyCount)
        {
            // 웨이브에 등장하는 적 랜덤 생성
            int enemyIndex = Random.Range(0, currentWave.enemyPrefabs.Length);
            // 적 오브젝트  생성
            GameObject clone = Instantiate(currentWave.enemyPrefabs[enemyIndex]);
            // 방금 생성된 적의 enemy 컴포넌트
            Enemy enemy = clone.GetComponent<Enemy>();
            // wayPoint 정보를 매개변수로 Setup() 호출
            enemy.Setup(this, wayPoints);
            // 생성된 적 리스트에 저장
            enemyList.Add(enemy);
            // 적 체력을 나타내는 Slider UI 생성 및 삭제
            SpawnEnemyHPSlider(clone);
            spawnEnemyCount++;
            // 대기
            yield return new WaitForSeconds(currentWave.spawnTime);
        }
    }

    public void DestoryEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        // 적이 목표지점까지 도착했을 때
        if(type == EnemyDestroyType.Arrive)
        {
            playerHP.TakeDamage(1);
        }
        // 적이 플레이어에 의헤 사망했을 때
        else if (type == EnemyDestroyType.Kill)
        {
            // 적의 종류에 따라 사망 시 골드 획득
            playerGold.CurrentGold += gold;
        }

        // 적이 사망할 때마다 적 숫자 감소 (UI용)  
        currentEnemyCount--;
        // 리스트에서 사망한 적 정보 삭제
        enemyList.Remove(enemy);
        //적 오브젝트 삭제
        Destroy(enemy.gameObject);
    }

    private void SpawnEnemyHPSlider(GameObject enemy)
    {
        GameObject sliderClone = Instantiate(enemyHPSliderPrefab);
        // Canvas을 부모로 설정 -> 그래야 보임
        sliderClone.transform.SetParent(canvasTransform);
        // 계층 설정으로 바뀐 크기를 다시 (1, 1, 1)로 설정
        sliderClone.transform.localScale = Vector3.one;

        // Slider UI가 쫒아다닐 대상을 본인으로 설정
        sliderClone.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);
        // Slider UI에 자신의 체력 정보를 표시하도록 설정
        sliderClone.GetComponent<EnemyHPViewer>().Setup(enemy.GetComponent<EnemyHP>());
    }
}
