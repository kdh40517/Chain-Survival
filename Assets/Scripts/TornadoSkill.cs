using UnityEngine;
using System.Collections;

public class TornadoSkill : SkillEffect
{
    [Header("회오리 고유 설정")]
    public TornadoProjectile projectilePrefab;
    public float baseSpeed = 4.0f;
    public float baseTickRate = 0.3f;

    private void Start()
    {
        StartCoroutine(FireTornadosRoutine());
    }

    IEnumerator FireTornadosRoutine()
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
        float finalScale = 1.0f + (chainLevel - 1) * 0.3f + myBonusSize;
        int totalProjectiles = chainLevel + extraProjectiles;

        Transform target = FindRandomEnemy();
        float baseAngle = 0f;

        if (target != null)
        {
            Vector2 targetDir = (target.position - transform.position).normalized;
            baseAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        }
        else
        {
            baseAngle = Random.Range(0f, 360f);
        }

        if (chainLevel >= 6)
        {
            totalProjectiles = 8;
            finalScale *= 2.5f;

            for (int i = 0; i < totalProjectiles; i++)
            {
                float angle = i * (360f / totalProjectiles);
                ShootTornado(angle, finalScale);
            }
            Debug.Log("[Tornado] 6체인 - 360도 전방향 발사");
        }
        else
        {
            float angleSpread = 20f;
            float startAngle = baseAngle - (angleSpread * (totalProjectiles - 1) / 2f);

            for (int i = 0; i < totalProjectiles; i++)
            {
                float angle = startAngle + (i * angleSpread);
                ShootTornado(angle, finalScale);
                yield return new WaitForSeconds(0.2f);
            }
        }

        Destroy(gameObject);
    }

    private void ShootTornado(float angle, float scale)
    {
        if (projectilePrefab == null) return;

        Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f);

        Vector3 spawnPos = transform.position + dir * 1.0f;

        TornadoProjectile tornado = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        Vector3 originalScale = projectilePrefab.transform.localScale;
        tornado.transform.localScale = originalScale * scale;

        tornado.Setup(skillDamage, knockbackPower * 1.5f, baseSpeed, baseTickRate, dir);

        Destroy(tornado.gameObject, 5f);
    }

    private Transform FindRandomEnemy()
    {
        EnemyController[] allEnemies = FindObjectsByType<EnemyController>(FindObjectsInactive.Exclude);
        if (allEnemies.Length == 0) return null;

        int randomIndex = Random.Range(0, allEnemies.Length);
        return allEnemies[randomIndex].transform;
    }
}