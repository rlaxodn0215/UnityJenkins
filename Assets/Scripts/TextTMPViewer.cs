using UnityEngine;
using TMPro;

public class TextTMPViewer : MonoBehaviour
{
    // �÷��̾� ü�� Text
    [SerializeField]
    private TextMeshProUGUI textPlayerHP;
    // �÷��̾� ü�� ����
    [SerializeField]
    private PlayerHP playerHP;

    // �÷��̾� ��� Text
    [SerializeField]
    private TextMeshProUGUI textPlayerGold;
    // �÷��̾� ��� ����
    [SerializeField]
    private PlayerGold playerGold;

    // ���̺� Text
    [SerializeField]
    private TextMeshProUGUI textWave;
    // ���̺� ����
    [SerializeField]
    private WaveSystem waveSystem;

    // ���� �� ���� Text
    [SerializeField]
    private TextMeshProUGUI textEnemyCount;
    // ���� �� ����
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
