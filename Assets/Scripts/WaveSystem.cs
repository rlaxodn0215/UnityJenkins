using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct Wave
{
    // ���� ���̺� �� ���� �ֱ�
    public float spawnTime;
    // ���� ���̺� �� ���� ����
    public int maxEnemyCount;
    // ���� ���̺� �� ���� ����
    public GameObject[] enemyPrefabs;
}

public class WaveSystem : MonoBehaviour
{
    [SerializeField]
    private Wave[] waves;
    [SerializeField]
    private EnemySpawner enemySpawner;
    // ���� ���̺� �ε���
    private int currentWaveIndex = -1;

    // ���̺��� ���� ���
    public int CurrenWave { get { return currentWaveIndex + 1; } }
    public int MaxWave { get { return waves.Length; } }

    public void StartWave()
    {
        if (currentWaveIndex == waves.Length-1)
        {
            SceneManager.LoadScene(2);
        }

        // ���� �ʿ� ���� ����, Wave�� ����������
        if (enemySpawner.EnemyList.Count == 0 && currentWaveIndex < waves.Length-1)
        {
            // ���̺� ����
            enemySpawner.StartWave(waves[++currentWaveIndex]);
        }
    }
}
