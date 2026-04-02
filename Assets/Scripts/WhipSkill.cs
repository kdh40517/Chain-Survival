using UnityEngine;
using System.Collections.Generic;
public class WhipSkill : SkillEffect
{
    public float sizeMultiplierPerLevel = 0.2f;
    public float duration = 0.2f;

    private float startTime;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    private HashSet<EnemyController> hitEnemies = new HashSet<EnemyController>();
    private Collider2D myCollider;

    [Header("채찍 고유 설정")]
    public float baseArcAngle = 120f;

    private void Start()
    {
        myCollider = GetComponent<Collider2D>();
        startTime = Time.time;

        // 1. 공용 주머니 스탯 챙기기
        int myBonusDamage = GameManager.instance.globalBonusDamage;
        float myBonusSize = GameManager.instance.globalBonusSize;
        float myBonusAngle = GameManager.instance.globalBonusAngle;
        float myBonusKnockback = 0f;

        // 2. 내 이름표(skillId)로 사물함 열어서 싹싹 긁어오기!
        if (GameManager.instance.specificBonusDamage.ContainsKey(skillId))
        {
            myBonusDamage += GameManager.instance.specificBonusDamage[skillId];
            myBonusSize += GameManager.instance.specificBonusSize[skillId];
            myBonusAngle += GameManager.instance.specificBonusAngle[skillId];
            myBonusKnockback += GameManager.instance.specificBonusKnockback[skillId];
        }

        // 3. 최종 스탯 진짜로 적용하기! 
        skillDamage += myBonusDamage;
        knockbackPower += myBonusKnockback; // 넉백 파워 증가 적용!

        // 크기 적용 
        float finalSizeMultiplier = 1.0f + (chainLevel - 1) * sizeMultiplierPerLevel + myBonusSize;
        transform.localScale = new Vector3(finalSizeMultiplier, finalSizeMultiplier, 1f);

        // 4. 적을 향해 조준!
        AimOnce();

        // 5. 회전 각도 적용! (baseArcAngle의 절반을 기준으로 잡고 내 보너스 각도를 더합니다)
        float swingAngle = (baseArcAngle / 2f) + myBonusAngle;

        // 6. 짝수/홀수에 따른 교차 스윙 세팅
        if (chainLevel % 2 == 0)
        {
            startRotation = transform.rotation * Quaternion.Euler(0f, 0f, swingAngle);
            targetRotation = transform.rotation * Quaternion.Euler(0f, 0f, -swingAngle);
        }
        else
        {
            startRotation = transform.rotation * Quaternion.Euler(0f, 0f, -swingAngle);
            targetRotation = transform.rotation * Quaternion.Euler(0f, 0f, swingAngle);
        }

        transform.rotation = startRotation;
        Destroy(gameObject, duration);
    }

    private void Update()
    {
        float progress = (Time.time - startTime) / duration;

        transform.rotation = Quaternion.Lerp(startRotation, targetRotation, progress);

        CheckHitsManually();
    }

    private void AimOnce()
    {
        EnemyController[] allEnemies = FindObjectsOfType<EnemyController>();
        if (allEnemies.Length == 0) return;

        Transform nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (EnemyController enemy in allEnemies)
        {
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

            transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
        }
    }
    private void CheckHitsManually()
    {
        if (myCollider == null) return;

        List<Collider2D> results = new List<Collider2D>();
        ContactFilter2D fillter = new ContactFilter2D();
        fillter.NoFilter();

        Physics2D.OverlapCollider(myCollider, fillter, results);

        foreach (Collider2D col in results)
        {
            EnemyController enemy = col.GetComponent<EnemyController>();

            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                enemy.TakeDamage(skillDamage, transform.position, knockbackPower);

                hitEnemies.Add(enemy);
            }
        }
    }
}
