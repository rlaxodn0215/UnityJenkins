using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // �� ü���� ��Ÿ���� Slider UI ������
    [SerializeField]
    private GameObject enemyHPSliderPrefab;
    // UI�� ǥ���ϴ� Canvas ������Ʈ�� Transform
    [SerializeField]
    private Transform canvasTransform;
    // ���� �������� �̵� ���
    [SerializeField]
    private Transform[] wayPoints;
    // �÷��̾��� ü�� ������Ʈ
    [SerializeField]
    private PlayerHP playerHP;
    // �÷��̾��� ��� ������Ʈ
    [SerializeField]
    private PlayerGold playerGold;
    // ���� ���̺� ����
    private Wave currentWave;
    // ���� ���̺꿡 �����ִ� �� ����
    private int currentEnemyCount;
    // ���� �ʿ� �ִ� ��� ���� ����
    private List<Enemy> enemyList;

    // ���� ������ ������ EnemySpawner���� �ϱ� ������ Set�� �ʿ� ����
    public List<Enemy> EnemyList { get { return enemyList; } }
    // ���� ���̺꿡 �����ִ� ��, �ִ� �� ����
    public int CurrentEnemyCount { get { return currentEnemyCount; } }
    public int MaxEnemyCount { get { return currentWave.maxEnemyCount; } }

    private void Awake()
    {
        // �� ����Ʈ �޸� �Ҵ�
        enemyList = new List<Enemy>();
    }

    public void StartWave(Wave wave)
    {
        // ���̺� ���� ����
        currentWave = wave;

        currentEnemyCount = currentWave.maxEnemyCount;

        // ���� ���̺� ����
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy()
    {
        // ���� ���̺꿡�� ������ �� ����
        int spawnEnemyCount = 0;

        while(spawnEnemyCount < currentWave.maxEnemyCount)
        {
            // ���̺꿡 �����ϴ� �� ���� ����
            int enemyIndex = Random.Range(0, currentWave.enemyPrefabs.Length);
            // �� ������Ʈ  ����
            GameObject clone = Instantiate(currentWave.enemyPrefabs[enemyIndex]);
            // ��� ������ ���� enemy ������Ʈ
            Enemy enemy = clone.GetComponent<Enemy>();
            // wayPoint ������ �Ű������� Setup() ȣ��
            enemy.Setup(this, wayPoints);
            // ������ �� ����Ʈ�� ����
            enemyList.Add(enemy);
            // �� ü���� ��Ÿ���� Slider UI ���� �� ����
            SpawnEnemyHPSlider(clone);
            spawnEnemyCount++;
            // ���
            yield return new WaitForSeconds(currentWave.spawnTime);
        }
    }

    public void DestoryEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        // ���� ��ǥ�������� �������� ��
        if(type == EnemyDestroyType.Arrive)
        {
            playerHP.TakeDamage(1);
        }
        // ���� �÷��̾ ���� ������� ��
        else if (type == EnemyDestroyType.Kill)
        {
            // ���� ������ ���� ��� �� ��� ȹ��
            playerGold.CurrentGold += gold;
        }

        // ���� ����� ������ �� ���� ���� (UI��)  
        currentEnemyCount--;
        // ����Ʈ���� ����� �� ���� ����
        enemyList.Remove(enemy);
        //�� ������Ʈ ����
        Destroy(enemy.gameObject);
    }

    private void SpawnEnemyHPSlider(GameObject enemy)
    {
        GameObject sliderClone = Instantiate(enemyHPSliderPrefab);
        // Canvas�� �θ�� ���� -> �׷��� ����
        sliderClone.transform.SetParent(canvasTransform);
        // ���� �������� �ٲ� ũ�⸦ �ٽ� (1, 1, 1)�� ����
        sliderClone.transform.localScale = Vector3.one;

        // Slider UI�� �i�ƴٴ� ����� �������� ����
        sliderClone.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);
        // Slider UI�� �ڽ��� ü�� ������ ǥ���ϵ��� ����
        sliderClone.GetComponent<EnemyHPViewer>().Setup(enemy.GetComponent<EnemyHP>());
    }
}
