using UnityEngine;

[CreateAssetMenu]
public class TowerTemplate : ScriptableObject
{
    [System.Serializable]
    public struct Weapon
    {
        // 보여지는 타워 이미지 (UI)
        public Sprite sprite;
        // 공격력
        public float damage;
        // 감속 퍼센트
        public float slow;
        // 공격력 증가 퍼센트
        public float buff;
        // 공격 속도
        public float rate;
        // 공격 범위
        public float range;
        // 필요 골드 (0레벨 : 건설, 1~레벨 : 업그레이드)
        public int cost;
        // 판매 골드
        public int sell;
    }

    // 타워 생성을 위한 프리팹
    public GameObject towerPrefab;
    // 임시 타워 프리팹
    public GameObject followTowerPrefab;
    // 레벨별 타워(무기) 정부
    public Weapon[] weapon;
}
