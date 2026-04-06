using UnityEngine;
using System.Collections;

public class FireballSkill : SkillEffect
{
    [Header("파이어볼 고유 설정")]
    public FireballProjectile projectilePrefab; // 💡 1단계에서 만든 불덩이 프리팹 넣기!
    public float baseExplosionRadius = 1.5f; // 기본 폭발 범위
    public float baseSpeed = 12f;

    private void Start()
    {
        StartCoroutine(FireShotsRoutine());
    }
    IEnumerator FireShotsRoutine()
    {
        // 1. 전용 사물함에서 스탯 긁어오기!
        int myBonusDamage = 0;
        float myBonusSize = 0f; // 파이어볼 크기 & 폭발 범위 증가
        int extraProjectiles = 0; // 발사 개수 증가 (chains 보너스로 씀)

        if (GameManager.instance.specificBonusDamage.ContainsKey(skillId))
        {
            myBonusDamage += GameManager.instance.specificBonusDamage[skillId];
            myBonusSize += GameManager.instance.specificBonusSize[skillId];
            extraProjectiles += GameManager.instance.specificBonusChains[skillId];
        }

        skillDamage += myBonusDamage;
        float finalScale = 3.0f + (chainLevel - 1) * 1.0f + myBonusSize; // 기본을 3.0으로 높이고 증가량도 1.0으로!
        float finalExplosionRadius = baseExplosionRadius * (finalScale / 3.0f);

        // 2. 발사 개수 결정
        int totalProjectiles = chainLevel + extraProjectiles;

        // 3. 적 조준하기
        Transform target = FindNearestEnemy();
        float baseAngle = 0f;

        if (target != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }

        // 🔥🔥🔥 [대망의 6체인 궁극기 로직] 🔥🔥🔥
        if (chainLevel >= 6)
        {
            // 궁극기는 한 번에 쾅! 터져야 멋있으므로 시간차 없이 즉시 8발 발사!
            totalProjectiles = 8;
            finalScale *= 4.0f;
            finalExplosionRadius *= 3.0f;

            for (int i = 0; i < totalProjectiles; i++)
            {
                float angle = i * (360f / totalProjectiles);
                ShootFireball(angle, finalScale, finalExplosionRadius);
            }
            Debug.Log("💥 [파이어볼 6체인] 360도 초거대 폭발 발사!!");
        }
        else
        {
            // 1~5체인: 부채꼴(산탄) 모양으로 발사!
            float angleSpread = 15f;
            float startAngle = baseAngle - (angleSpread * (totalProjectiles - 1) / 2f);

            for (int i = 0; i < totalProjectiles; i++)
            {
                float angle = startAngle + (i * angleSpread);
                ShootFireball(angle, finalScale, finalExplosionRadius);

                // ⭐ 핵심: 여기서 0.1초씩 쉬어주면서 쏩니다! (두다다당!)
                yield return new WaitForSeconds(0.1f);
            }
        }

        // 다 쐈으면 발사대는 쿨하게 퇴장!
        Destroy(gameObject);
    }

    private void ShootFireball(float angle, float scale, float expRadius)
    {
        if (projectilePrefab == null) return;

        // 불덩이 소환!
        FireballProjectile fb = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0f, 0f, angle));

        // 크기 키우기 (유저님이 원한 Size 연동)
        fb.transform.localScale = new Vector3(scale, scale, 1f);

        // 스탯 전달 (데미지, 넉백, 폭발범위, 속도)
        fb.Setup(skillDamage, knockbackPower, expRadius, baseSpeed);

        // 불덩이가 화면 밖으로 영원히 날아가지 않게 3초 뒤 자동 폭파
        Destroy(fb.gameObject, 3f);
    }

    // (기존에 쓰던 가장 가까운 적 찾는 함수)
    private Transform FindNearestEnemy()
    {
        EnemyController[] allEnemies = FindObjectsByType<EnemyController>(FindObjectsInactive.Exclude);
        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (EnemyController enemy in allEnemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = enemy.transform;
            }
        }
        return nearest;
    }
}