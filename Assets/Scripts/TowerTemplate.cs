using UnityEngine;

[CreateAssetMenu]
public class TowerTemplate : ScriptableObject
{
    [System.Serializable]
    public struct Weapon
    {
        // �������� Ÿ�� �̹��� (UI)
        public Sprite sprite;
        // ���ݷ�
        public float damage;
        // ���� �ۼ�Ʈ
        public float slow;
        // ���ݷ� ���� �ۼ�Ʈ
        public float buff;
        // ���� �ӵ�
        public float rate;
        // ���� ����
        public float range;
        // �ʿ� ��� (0���� : �Ǽ�, 1~���� : ���׷��̵�)
        public int cost;
        // �Ǹ� ���
        public int sell;
    }

    // Ÿ�� ������ ���� ������
    public GameObject towerPrefab;
    // �ӽ� Ÿ�� ������
    public GameObject followTowerPrefab;
    // ������ Ÿ��(����) ����
    public Weapon[] weapon;
}
