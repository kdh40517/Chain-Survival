using System.Collections.Generic;
using UnityEngine;

public class FireSkill : SkillEffect
{
    private ParticleSystem myParticleSystem;
    public float sizeMultiplierPerLevel = 1.0f;
    public float turnSpeed = 8f;
    private void Awake()
    {
        myParticleSystem = GetComponent<ParticleSystem>();
    }
    private void Start()
    {
        // 1. 공용 주머니(글로벌) 스탯 먼저 챙기기
        int myBonusDamage = 0;
        float myBonusSize = 0f;
        float myBonusDuration = 0f;
        float myBonusTickInterval = 0f;

        // 2. 내 이름표(skillId)로 사물함 열어서 싹싹 긁어오기!
        if (GameManager.instance.specificBonusDamage.ContainsKey(skillId))
        {
            myBonusDamage += GameManager.instance.specificBonusDamage[skillId];
            myBonusSize += GameManager.instance.specificBonusSize[skillId];
            myBonusDuration += GameManager.instance.specificBonusDuration[skillId];
            myBonusTickInterval += GameManager.instance.specificBonusTickRate[skillId];
        }

        // 3. 최종 스탯 적용!
        skillDamage += myBonusDamage;
        float finalSizeScale = 1.0f + (chainLevel - 1) * sizeMultiplierPerLevel + myBonusSize;
        float finalDuration = 1.5f + myBonusDuration;

        // [꿀팁] 타격 간격은 작아질수록 빨리 때립니다! 보너스(myBonusTickInterval)를 빼줍니다. 
        // Mathf.Max를 써서 아무리 강화해도 0.05초 이하로는 안 내려가게 방어막을 칩니다.
        damageTickRate = Mathf.Max(0.05f, 0.25f - myBonusTickInterval);

        // 4. 눈에 보이는 크기(파티클 & 콜라이더) 키우기
        if (myParticleSystem != null)
        {
            var mainModule = myParticleSystem.main;
            mainModule.startSizeMultiplier = finalSizeScale;
        }
        transform.localScale = new Vector3(finalSizeScale, finalSizeScale, 1f);

        Debug.Log($"[{skillId}] 화염 폭발!! 뎀:{skillDamage} | 크기:{finalSizeScale} | 지속:{finalDuration}초 | 타격간격:{damageTickRate}초");

        // 적용된 지속시간이 끝나면 깔끔하게 소멸!
        Destroy(gameObject, finalDuration);
    }
    private void Update()
    {
        AimAtNearestEnemy();
    }
    private void AimAtNearestEnemy()
    {
        EnemyController[] allEnemies = FindObjectsByType<EnemyController>(FindObjectsInactive.Exclude);
        if (allEnemies.Length == 0) return;
        Transform nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (EnemyController enemy in allEnemies)
        {
            if (enemy == null || enemy.isDead) continue;

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }
        if (nearestEnemy != null)
        {
            Vector2 dir = (nearestEnemy.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float offsetAngle = (chainLevel - 1) * 15f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle + 90f + offsetAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }
    private Dictionary<EnemyController, float> lastHitTimes = new Dictionary<EnemyController, float>();
    public float damageTickRate = 0.25f;
    private void OnTriggerStay2D(Collider2D collision)
    {
        EnemyController enemy = collision.GetComponent<EnemyController>();

        if (enemy != null)
        {
            if (!lastHitTimes.ContainsKey(enemy) || Time.time - lastHitTimes[enemy] >= damageTickRate)
            {
                enemy.TakeDamage(skillDamage, transform.position, knockbackPower);
                lastHitTimes[enemy] = Time.time;
            }
        }
    }
}
