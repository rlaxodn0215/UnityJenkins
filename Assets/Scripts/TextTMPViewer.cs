using UnityEngine;
using TMPro;

public class TextTMPViewer : MonoBehaviour
{
    // 플레이어 체력 Text
    [SerializeField]
    private TextMeshProUGUI textPlayerHP;
    // 플레이어 체력 정보
    [SerializeField]
    private PlayerHP playerHP;

    // 플레이어 골드 Text
    [SerializeField]
    private TextMeshProUGUI textPlayerGold;
    // 플레이어 골드 정보
    [SerializeField]
    private PlayerGold playerGold;

    // 웨이브 Text
    [SerializeField]
    private TextMeshProUGUI textWave;
    // 웨이브 정보
    [SerializeField]
    private WaveSystem waveSystem;

    // 현재 적 숫자 Text
    [SerializeField]
    private TextMeshProUGUI textEnemyCount;
    // 현재 적 정보
    [SerializeField]
    private EnemySpawner enemySpawner;

    private void Update()
    {
        textPlayerHP.text = playerHP.CurrentHP + " / " + playerHP.MaxHP;
        textPlayerGold.text = playerGold.CurrentGold.ToString();
        textWave.text = waveSystem.CurrenWave + " / " + waveSystem.MaxWave;
        textEnemyCount.text = enemySpawner.CurrentEnemyCount + " / " + enemySpawner.MaxEnemyCount;
    }

}
