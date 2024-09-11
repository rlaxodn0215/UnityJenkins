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
    // 타워 정보
    [SerializeField]
    private TowerTemplate towerTemplate;
    // 발사체 생성 위치
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private WeaponType weaponType;

    [Header("Cannon")]
    // 발사체 프리팹
    [SerializeField]
    private GameObject projectilePrefab;

    [Header("Laser")]
    // 레이저로 사용되는 선
    [SerializeField]
    private LineRenderer lineRenderer;
    // 타격 효과
    [SerializeField]
    private Transform hitEffect;
    // 광선에 부딪치는 레이어 설정
    [SerializeField]
    private LayerMask targetLayer;

    // 타워 레벨
    private int level = 0;
    // 타워 무기 상태
    private WeaponState weaponState = WeaponState.SearchTarget;
    // 공격 대상
    private Transform attackTarget = null;
    // 타워 오브젝트 이미지 변경용
    private SpriteRenderer spriteRenderer;
    private TowerSpawner towerSpawner;
    // 게임에 존재하는 적 정보 획득용
    private EnemySpawner enemySpawner;
    // 플레이어 골드 정보
    private PlayerGold playerGold;
    // 현재 타워가 배치되어있는 타일
    private Tile ownerTile;

    // 버프에 의해 추가된 데미지
    private float addedDamage;
    // 버프를 받는지 여부 설정 (0 : 버프X, 1~ : 버프0)
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

        // 무기 속성이 캐논, 레이져일 때
        if (weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser)
        {
            // 최초 상태를 SearchTarget으로 설정
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
            // target을 공격하는게 가능한지 검사
            if(!IsPossibleToAttackTarget())
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // 공격 속도 적용
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            // 캐논 공격! - 발사체 생성
            SpawnProjectile();
        }
    }

    private IEnumerator TryAttackLaser()
    {
        // 레이저, 레이저 타격 효과 활성화
        EnableLaser();

        while(true)
        {
            // target공격 가능한지 검사
            if(IsPossibleToAttackTarget() == false)
            {
                // 레이저, 레이저 타격 효과 비활성화
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // 레이져 공격
            SpawnLaser();

            yield return null;
        }
    }

    public void OnBuffAroundTower()
    {
        // 현재 멥에 배치된 Tower 태그 검색
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for(int i = 0; i < towers.Length; i++)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            // 이미 버프를 받고 있고, 현재 버프 타워의 레벨보다 높은 버프이면 패스
            if(weapon.BuffLevel > Level)
            {
                continue;
            }

            // 현재 버프 타워와 다른 타워의 거리를 검사해서 범위 안에 타워가 있으면
            if(Vector3.Distance(weapon.transform.position, transform.position) <= towerTemplate.weapon[level].range)
            {
                // 공격이 가능한 캐논, 레이져 타워 이면
                if(weapon.WeaponType == WeaponType.Cannon || weapon.WeaponType == WeaponType.Laser)
                {
                    // 공격력 증가
                    weapon.AddedDamage = weapon.Damage * (towerTemplate.weapon[level].buff);
                    // 타워가 받고 있는 버프 레벨 설정
                    weapon.BuffLevel = Level;
                }
            }
        }
    }

    private Transform FindClosestAttackTarget()
    {
        // 제일 가까히 있는 적 찾기
        float closestDistSqr = Mathf.Infinity;
        // EnemySpawner의 EnemyList에 있는 모든 적 검사
        for (int i = 0; i < enemySpawner.EnemyList.Count; i++)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
            // 공격범위 내에 있고, 현재까지 검사한 적보다 거리가 가까우면
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
        // target이 있는지 검사
        if (attackTarget == null) return false;

        // target이 공격 범위 안에 있는지 검사 -> 벗어나면 새로운 적 탐색
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
        // 생성된 발사체에 타겟 정보 전달
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

        // 같은 방향으로 여러개의 광선을 쏴서 그중 현재 attackTarget과 동일한 오브젝트 검출
        for(int i = 0; i < hit.Length; i++)
        {
            if(hit[i].transform == attackTarget)
            {
                // 선의 시작지점
                lineRenderer.SetPosition(0, spawnPoint.position);
                // 선의 목표지점
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                // 타격 효과 위치 설정
                hitEffect.position = hit[i].point;
                // 적 체력만큼 감소 (1초에 damage만큼 감소)
                float damage = towerTemplate.weapon[level].damage + AddedDamage;
                attackTarget.GetComponent<EnemyHP>().TakeDamage(damage * Time.deltaTime);
            }
        }
    }

    public bool Upgrade()
    {
        // 타워 업그레이드에 필요한 골드가 충분한지 검사
        if(playerGold.CurrentGold < towerTemplate.weapon[level+1].cost)
        {
            return false;
        }

        // 타워 레벨 중가
        level++;
        // 타워 외형 변경
        spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
        // 골드 차감
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;

        // 무기 속성이 레이져이면
        if(weaponType == WeaponType.Laser)
        {
            // 레벨에 따라 레이져 굵기 설정
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }

        // 버프 타워가 업그레이드 될 때 모든 버프 타워의 버프 효과 갱신
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
