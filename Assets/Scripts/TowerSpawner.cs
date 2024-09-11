/*
 * File : TowerSpawner.cs
 * Desc
 *  : Ÿ�� ���� ����
 *  
 * Functions
 *  : SpawnTower(Transform) - Transform ��ġ�� Ÿ�� ���� 
 */

using UnityEngine;
using System.Collections;

public class TowerSpawner : MonoBehaviour
{
    // Ÿ�� ����
    [SerializeField]
    private TowerTemplate[] towerTemplate;
    // �� ���� ���
    [SerializeField]
    private EnemySpawner enemySpawner;
    // Ÿ�� �Ǽ� �� ��� ����
    [SerializeField]
    private PlayerGold playerGold;
    // �ý��� �޼��� ���
    [SerializeField]
    private SystemTextViewer systemTextViewer;
    // Ÿ�� �Ǽ� ��ư�� �������� üũ
    private bool isOnTowerButton = false;
    // �ӽ� Ÿ�� ��� �Ϸ� �� ������ ���� �����ϴ� ����
    private GameObject followTowerClone = null;
    // Ÿ�� �Ӽ�
    private int towerType;

    public void ReadyToSpawnTower(int type)
    {
        towerType = type;
        // ��ư �ߺ� �Է� ����
        if (isOnTowerButton) return;
        // Ÿ�� �Ǽ� ���� ���� Ȯ��
        // Ÿ�� �Ǽ��� ��ŭ ���� ������ Ÿ�� �Ǽ� X
        if(towerTemplate[towerType].weapon[0].cost > playerGold.CurrentGold)
        {
            // ��尡 �����ؼ� Ÿ�� �Ǽ��� �Ұ����ϴٰ� Ǯ��
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }

        // Ÿ�� ���� ��ư�� �����ٰ� ����
        isOnTowerButton = true;
        // ���콺�� ����ٴϴ� �ӽ� Ÿ�� ����
        followTowerClone = Instantiate(towerTemplate[towerType].followTowerPrefab);
        // Ÿ�� �Ǽ��� ����� �� �ִ� �ڷ�ƾ �Լ� ����
        StartCoroutine("OnTowerCancelSystem");
    }

    public void SpawnTower(Transform tileTransform)
    {
        // Ƽ�� �Ǽ� ��ư�� ������ ���� Ÿ�� �Ǽ� ����
        if (!isOnTowerButton) return;

        Tile tile = tileTransform.GetComponent<Tile>();
        // Ÿ�� �Ǽ� ���� ���� Ȯ��
        if (tile.IsBuildTower)
        {
            // ���� ��ġ�� Ÿ�� �Ǽ��� �Ұ���
            systemTextViewer.PrintText(SystemType.Build);
            return;
        }

        // �ٽ� Ÿ�� �Ǽ� ��ư�� ������ Ÿ���� �Ǽ��ϵ��� ���� ����
        isOnTowerButton = false;
        // Ÿ���� �Ǽ��Ǿ� �������� ����
        tile.IsBuildTower = true;
        playerGold.CurrentGold -= towerTemplate[towerType].weapon[0].cost;

        // ������ Ÿ���� ��ġ�� Ÿ�� �Ǽ�(Ÿ�� ���� z�� -1�� ��ġ�� ��ġ)
        Vector3 position = tileTransform.position + Vector3.back;
        // ������ Ÿ���� ��ġ�� Ÿ�� ����
        GameObject clone = Instantiate(towerTemplate[towerType].towerPrefab, position, Quaternion.identity);
        // Ÿ�� ���⿡ enemySpawner ���� ����
        clone.GetComponent<TowerWeapon>().Setup(this, enemySpawner, playerGold, tile);

        // ���� ��ġ�Ǵ� Ÿ���� ���� Ÿ�� �ֺ��� ��ġ�� ���, ���� ȿ���� ���� �� �ֵ��� ��� ���� Ÿ���� ���� ȿ�� ����
        OnBuffAllBuffTowers();

        // ����ٴϴ� �ӽ� Ÿ�� ����
        Destroy(followTowerClone);
        // Ÿ�� �Ǽ��� ����� �� �ִ� �ڷ�ƾ �Լ� ����
        StopCoroutine("OnTowerCancelSystem");
    }

    private IEnumerator OnTowerCancelSystem()
    {
        while(true)
        {
            // ESCŰ �Ǵ� ���콺 ������ ��ư�� ������ �� Ÿ�� �Ǽ� ���
            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                isOnTowerButton = false;
                // �ӽ� Ÿ�� ����
                Destroy(followTowerClone);
                break;
            }

            yield return null;
        }
    }

    public void OnBuffAllBuffTowers()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for(int i = 0; i < towers.Length; i++)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            if(weapon.WeaponType == WeaponType.Buff)
            {
                weapon.OnBuffAroundTower();
            }
        }
    }
}
