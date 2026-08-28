using UnityEngine;
using System.Collections;

public class FireballSkill : SkillEffect
{
    [Header("파이어볼 고유 설정")]
    public FireballProjectile projectilePrefab;
    public float baseExplosionRadius = 1.5f;
    public float baseSpeed = 12f;

    private void Start()
    {
        StartCoroutine(FireShotsRoutine());
    }
    IEnumerator FireShotsRoutine()
    {
        int myBonusDamage = 0;
        float myBonusSize = 0f;
        int extraProjectiles = 0;

        if (GameManager.instance.specificBonusDamage.ContainsKey(skillId))
        {
            myBonusDamage += GameManager.instance.specificBonusDamage[skillId];
            myBonusSize += GameManager.instance.specificBonusSize[skillId];
            extraProjectiles += GameManager.instance.specificBonusChains[skillId];
        }

        skillDamage += myBonusDamage;
        float finalScale = 3.0f + (chainLevel - 1) * 1.0f + myBonusSize;
        float finalExplosionRadius = baseExplosionRadius * (finalScale / 3.0f);

        int totalProjectiles = chainLevel + extraProjectiles;

        Transform target = FindNearestEnemy();
        float baseAngle = 0f;

        if (target != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }

        if (chainLevel >= 6)
        {
            totalProjectiles = 8;
            finalScale *= 4.0f;
            finalExplosionRadius *= 3.0f;

            for (int i = 0; i < totalProjectiles; i++)
            {
                float angle = i * (360f / totalProjectiles);
                ShootFireball(angle, finalScale, finalExplosionRadius);
            }
            Debug.Log("[Fireball] 6체인 - 360도 전방향 발사");
        }
        else
        {
            float angleSpread = 15f;
            float startAngle = baseAngle - (angleSpread * (totalProjectiles - 1) / 2f);

            for (int i = 0; i < totalProjectiles; i++)
            {
                float angle = startAngle + (i * angleSpread);
                ShootFireball(angle, finalScale, finalExplosionRadius);

                yield return new WaitForSeconds(0.1f);
            }
        }

        Destroy(gameObject);
    }

    private void ShootFireball(float angle, float scale, float expRadius)
    {
        if (projectilePrefab == null) return;

        FireballProjectile fb = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0f, 0f, angle));

        fb.transform.localScale = new Vector3(scale, scale, 1f);

        fb.Setup(skillDamage, knockbackPower, expRadius, baseSpeed);

        Destroy(fb.gameObject, 3f);
    }

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