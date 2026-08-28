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
        int myBonusDamage = 0;
        float myBonusSize = 0f;
        float myBonusDuration = 0f;
        float myBonusTickInterval = 0f;

        if (GameManager.instance.specificBonusDamage.ContainsKey(skillId))
        {
            myBonusDamage += GameManager.instance.specificBonusDamage[skillId];
            myBonusSize += GameManager.instance.specificBonusSize[skillId];
            myBonusDuration += GameManager.instance.specificBonusDuration[skillId];
            myBonusTickInterval += GameManager.instance.specificBonusTickRate[skillId];
        }

        skillDamage += myBonusDamage;
        float finalSizeScale = 1.0f + (chainLevel - 1) * sizeMultiplierPerLevel + myBonusSize;
        float finalDuration = 1.5f + myBonusDuration;

        damageTickRate = Mathf.Max(0.05f, 0.25f - myBonusTickInterval);

        if (myParticleSystem != null)
        {
            var mainModule = myParticleSystem.main;
            mainModule.startSizeMultiplier = finalSizeScale;
        }
        transform.localScale = new Vector3(finalSizeScale, finalSizeScale, 1f);

        Debug.Log($"[{skillId}] damage={skillDamage} scale={finalSizeScale} duration={finalDuration} tickRate={damageTickRate}");

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
