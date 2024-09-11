using System.Collections;
using UnityEngine;

public enum WeaponType
{
    Cannon,
    Laser,
    Slow,
    Buff
}

public enum WeaponState
{
    SearchTarget,
    TryAttackCannon,
    TryAttackLaser
}

public class TowerWeapon : MonoBehaviour
{
    [Header("Commons")]
    // Ÿ�� ����
    [SerializeField]
    private TowerTemplate towerTemplate;
    // �߻�ü ���� ��ġ
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private WeaponType weaponType;

    [Header("Cannon")]
    // �߻�ü ������
    [SerializeField]
    private GameObject projectilePrefab;

    [Header("Laser")]
    // �������� ���Ǵ� ��
    [SerializeField]
    private LineRenderer lineRenderer;
    // Ÿ�� ȿ��
    [SerializeField]
    private Transform hitEffect;
    // ������ �ε�ġ�� ���̾� ����
    [SerializeField]
    private LayerMask targetLayer;

    // Ÿ�� ����
    private int level = 0;
    // Ÿ�� ���� ����
    private WeaponState weaponState = WeaponState.SearchTarget;
    // ���� ���
    private Transform attackTarget = null;
    // Ÿ�� ������Ʈ �̹��� �����
    private SpriteRenderer spriteRenderer;
    private TowerSpawner towerSpawner;
    // ���ӿ� �����ϴ� �� ���� ȹ���
    private EnemySpawner enemySpawner;
    // �÷��̾� ��� ����
    private PlayerGold playerGold;
    // ���� Ÿ���� ��ġ�Ǿ��ִ� Ÿ��
    private Tile ownerTile;

    // ������ ���� �߰��� ������
    private float addedDamage;
    // ������ �޴��� ���� ���� (0 : ����X, 1~ : ����0)
    private int buffLevel;

    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate => towerTemplate.weapon[level].rate;
    public float Range => towerTemplate.weapon[level].range;
    public int Level => level + 1;
    public int MaxLevel => towerTemplate.weapon.Length;
    public float Slow => towerTemplate.weapon[level].slow;
    public float Buff => towerTemplate.weapon[level].buff;
    public WeaponType WeaponType => weaponType;
    public float AddedDamage
    {
        set => addedDamage = Mathf.Max(0, value);
        get => addedDamage;
    }
    public int BuffLevel
    {
        set => buffLevel = Mathf.Max(0, value);
        get => buffLevel;
    }

    public void Setup(TowerSpawner towerSpawner, EnemySpawner enemySpawner, PlayerGold playerGold, Tile ownerTile)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.towerSpawner = towerSpawner;
        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;
        this.ownerTile = ownerTile;

        ChangeState(WeaponState.SearchTarget);

        // ���� �Ӽ��� ĳ��, �������� ��
        if (weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser)
        {
            // ���� ���¸� SearchTarget���� ����
            ChangeState(WeaponState.SearchTarget);
        }
    }

    public void ChangeState(WeaponState newState)
    {
        StopCoroutine(weaponState.ToString());
        weaponState = newState;
        StartCoroutine(weaponState.ToString());
    }

    private void Update()
    {
        if(attackTarget != null)
        {
            RotateToTarget();
        }
    }

    private void RotateToTarget()
    {
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;
        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    private IEnumerator SearchTarget()
    {
        while(true)
        {
            attackTarget = FindClosestAttackTarget();

            if (attackTarget != null)
            {
                if(weaponType == WeaponType.Cannon)
                {
                    ChangeState(WeaponState.TryAttackCannon);
                }
                else if (weaponType == WeaponType.Laser)
                {
                    ChangeState(WeaponState.TryAttackLaser);
                }
            }

            yield return null;
        }
    }

    private IEnumerator TryAttackCannon()
    {
        while(true)
        {
            // target�� �����ϴ°� �������� �˻�
            if(!IsPossibleToAttackTarget())
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // ���� �ӵ� ����
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            // ĳ�� ����! - �߻�ü ����
            SpawnProjectile();
        }
    }

    private IEnumerator TryAttackLaser()
    {
        // ������, ������ Ÿ�� ȿ�� Ȱ��ȭ
        EnableLaser();

        while(true)
        {
            // target���� �������� �˻�
            if(IsPossibleToAttackTarget() == false)
            {
                // ������, ������ Ÿ�� ȿ�� ��Ȱ��ȭ
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // ������ ����
            SpawnLaser();

            yield return null;
        }
    }

    public void OnBuffAroundTower()
    {
        // ���� �㿡 ��ġ�� Tower �±� �˻�
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for(int i = 0; i < towers.Length; i++)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            // �̹� ������ �ް� �ְ�, ���� ���� Ÿ���� �������� ���� �����̸� �н�
            if(weapon.BuffLevel > Level)
            {
                continue;
            }

            // ���� ���� Ÿ���� �ٸ� Ÿ���� �Ÿ��� �˻��ؼ� ���� �ȿ� Ÿ���� ������
            if(Vector3.Distance(weapon.transform.position, transform.position) <= towerTemplate.weapon[level].range)
            {
                // ������ ������ ĳ��, ������ Ÿ�� �̸�
                if(weapon.WeaponType == WeaponType.Cannon || weapon.WeaponType == WeaponType.Laser)
                {
                    // ���ݷ� ����
                    weapon.AddedDamage = weapon.Damage * (towerTemplate.weapon[level].buff);
                    // Ÿ���� �ް� �ִ� ���� ���� ����
                    weapon.BuffLevel = Level;
                }
            }
        }
    }

    private Transform FindClosestAttackTarget()
    {
        // ���� ������ �ִ� �� ã��
        float closestDistSqr = Mathf.Infinity;
        // EnemySpawner�� EnemyList�� �ִ� ��� �� �˻�
        for (int i = 0; i < enemySpawner.EnemyList.Count; i++)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
            // ���ݹ��� ���� �ְ�, ������� �˻��� ������ �Ÿ��� ������
            if (distance <= towerTemplate.weapon[level].range && distance < closestDistSqr)
            {
                closestDistSqr = distance;
                attackTarget = enemySpawner.EnemyList[i].transform;
            }
        }

        return attackTarget;
    }

    private bool IsPossibleToAttackTarget()
    {
        // target�� �ִ��� �˻�
        if (attackTarget == null) return false;

        // target�� ���� ���� �ȿ� �ִ��� �˻� -> ����� ���ο� �� Ž��
        float distance = Vector3.Distance(attackTarget.position, transform.position);
        if (distance > towerTemplate.weapon[level].range)
        {
            attackTarget = null;
            return false;
        }

        return true;
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        // ������ �߻�ü�� Ÿ�� ���� ����
        float damage = towerTemplate.weapon[level].damage + AddedDamage;
        clone.GetComponent<Projectile>().Setup(attackTarget, damage);
    }

    private void EnableLaser()
    {
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
    }

    private void DisableLaser()
    {
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);
    }

    private void SpawnLaser()
    {
        Vector3 direction = attackTarget.position - spawnPoint.position;
        RaycastHit2D[] hit = Physics2D.RaycastAll(spawnPoint.position, direction, towerTemplate.weapon[level].range, targetLayer);

        // ���� �������� �������� ������ ���� ���� ���� attackTarget�� ������ ������Ʈ ����
        for(int i = 0; i < hit.Length; i++)
        {
            if(hit[i].transform == attackTarget)
            {
                // ���� ��������
                lineRenderer.SetPosition(0, spawnPoint.position);
                // ���� ��ǥ����
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                // Ÿ�� ȿ�� ��ġ ����
                hitEffect.position = hit[i].point;
                // �� ü�¸�ŭ ���� (1�ʿ� damage��ŭ ����)
                float damage = towerTemplate.weapon[level].damage + AddedDamage;
                attackTarget.GetComponent<EnemyHP>().TakeDamage(damage * Time.deltaTime);
            }
        }
    }

    public bool Upgrade()
    {
        // Ÿ�� ���׷��̵忡 �ʿ��� ��尡 ������� �˻�
        if(playerGold.CurrentGold < towerTemplate.weapon[level+1].cost)
        {
            return false;
        }

        // Ÿ�� ���� �߰�
        level++;
        // Ÿ�� ���� ����
        spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
        // ��� ����
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;

        // ���� �Ӽ��� �������̸�
        if(weaponType == WeaponType.Laser)
        {
            // ������ ���� ������ ���� ����
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }

        // ���� Ÿ���� ���׷��̵� �� �� ��� ���� Ÿ���� ���� ȿ�� ����
        towerSpawner.OnBuffAllBuffTowers();

        return true;
    }

    public void Sell()
    {
        playerGold.CurrentGold += towerTemplate.weapon[level].sell;
        ownerTile.IsBuildTower = false;
        Destroy(gameObject);
    }
}
