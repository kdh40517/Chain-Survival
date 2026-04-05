using UnityEngine;
using System.Collections.Generic;

public class WhipSkill : SkillEffect
{
    public float sizeMultiplierPerLevel = 0.2f;
    public float duration = 0.3f; // 💡 만든 애니메이션의 실제 길이에 맞춰주세요!

    private float startTime;
    private HashSet<EnemyController> hitEnemies = new HashSet<EnemyController>();
    private Collider2D myCollider;
    private SpriteRenderer sr; // 💡 애니메이션 좌우 반전을 위해 추가

    private void Start()
    {
        myCollider = GetComponent<Collider2D>();
        // 자식(Whip_Slash)에 있는 SpriteRenderer를 찾아옵니다.
        sr = GetComponentInChildren<SpriteRenderer>(); 
        startTime = Time.time;

        // 1~3. 스탯 긁어오기 (유저님 코드 완벽함!)
        int myBonusDamage = GameManager.instance.globalBonusDamage;
        float myBonusSize = GameManager.instance.globalBonusSize;
        float myBonusKnockback = 0f;

        if (GameManager.instance.specificBonusDamage.ContainsKey(skillId))
        {
            myBonusDamage += GameManager.instance.specificBonusDamage[skillId];
            myBonusSize += GameManager.instance.specificBonusSize[skillId];
            myBonusKnockback += GameManager.instance.specificBonusKnockback[skillId];
        }

        skillDamage += myBonusDamage;
        knockbackPower += myBonusKnockback;

        float finalSizeMultiplier = 1.0f + (chainLevel - 1) * sizeMultiplierPerLevel + myBonusSize;
        transform.localScale = new Vector3(finalSizeMultiplier, finalSizeMultiplier, 1f);

        // 4. 적을 향해 조준! (한 번만 딱 조준합니다)
        AimOnce();

        // 5. 짝수/홀수에 따른 교차 스윙 (물리적 회전 대신 이미지 뒤집기!)
        if (chainLevel % 2 == 0 && sr != null)
        {
            // 이펙트 모양에 따라 flipX 또는 flipY를 선택하세요! (휘두르는 방향이 반대로 보임)
            sr.flipY = true; 
        }

        // 애니메이션 재생 시간(duration)이 끝나면 칼같이 삭제!
        Destroy(gameObject, duration);
    }

    private void Update()
    {
        // 💡 궤적 애니메이션 자체가 휘두르는 모습을 다 보여주므로 Lerp 회전은 과감히 삭제했습니다!
        CheckHitsManually();
    }

    private void AimOnce()
    {
        // 유저님 코드 그대로 유지! (아주 훌륭합니다)
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
            
            // 이펙트 원본 이미지가 어디를 보고 있느냐에 따라 + 90f 보정은 조절해주세요.
            transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
        }
    }

    private void CheckHitsManually()
    {
        // 유저님 코드 그대로 유지! (완벽합니다)
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