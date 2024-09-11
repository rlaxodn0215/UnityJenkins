using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct Wave
{
    // 현재 웨이브 적 생성 주기
    public float spawnTime;
    // 현재 웨이브 적 등장 숫자
    public int maxEnemyCount;
    // 현재 웨이브 적 등장 종류
    public GameObject[] enemyPrefabs;
}

public class WaveSystem : MonoBehaviour
{
    [SerializeField]
    private Wave[] waves;
    [SerializeField]
    private EnemySpawner enemySpawner;
    // 현재 웨이브 인덱스
    private int currentWaveIndex = -1;

    // 웨이브위 정보 출력
    public int CurrenWave { get { return currentWaveIndex + 1; } }
    public int MaxWave { get { return waves.Length; } }

    public void StartWave()
    {
        if (currentWaveIndex == waves.Length-1)
        {
            SceneManager.LoadScene(2);
        }

        // 현재 맵에 적이 없고, Wave가 남아있으면
        if (enemySpawner.EnemyList.Count == 0 && currentWaveIndex < waves.Length-1)
        {
            // 웨이브 시작
            enemySpawner.StartWave(waves[++currentWaveIndex]);
        }
    }
}
