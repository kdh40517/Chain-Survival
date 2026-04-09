using UnityEngine;
using System.Collections;

public class ShockwaveSkill : SkillEffect
{
    [Header("충격파 고유 설정")]
    public float baseRadius = 3.0f;     // 기본 폭발 반경
    public float expandDuration = 0.2f; // 0.2초 만에 "쾅!" 하고 터짐 (속도)

    private void Start()
    {
        // 💡 충격파는 플레이어 몸에서 터져야 하므로 위치를 플레이어에 맞춥니다.
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            transform.position = player.transform.position;
        }
        transform.SetParent(null);

        StartCoroutine(ShockwaveRoutine());
    }

    IEnumerator ShockwaveRoutine()
    {
        // 1. 사물함에서 스탯 긁어오기
        int myBonusDamage = 0;
        float myBonusSize = 0f;

        if (GameManager.instance.specificBonusDamage.ContainsKey(skillId))
        {
            myBonusDamage += GameManager.instance.specificBonusDamage[skillId];
            myBonusSize += GameManager.instance.specificBonusSize[skillId];
        }

        skillDamage += myBonusDamage;

        // 최종 반경 = 기본 반경 + 체인당 0.5씩 증가 + 보너스 사이즈
        float finalRadius = baseRadius + (chainLevel * 0.5f) + myBonusSize;

        // 💡 충격파의 핵심은 넉백! 체인이 오를수록 더 멀리 날려버립니다.
        float finalKnockback = knockbackPower * (1f + chainLevel * 0.2f);

        // 🚨 [대망의 6체인 궁극기 로직] 🚨
        if (chainLevel >= 6)
        {
            finalRadius *= 3.0f; // 범위 3배!! (화면 전체 폭발)
            skillDamage *= 2;    // 데미지도 2배!
            finalKnockback *= 2.0f;
            Debug.Log("💥 [충격파 6체인] 화면 붕괴 붕괴!! 싹 다 밀쳐냄!!");
        }

        // 2. 데미지 즉시 판정 (이게 제일 중요! ⭐)
        // 콜라이더가 커지길 기다리지 않고, 폭발 범위 안의 모든 적을 한 번에 긁어와서 때립니다.
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, finalRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(skillDamage, transform.position, finalKnockback);
                }
            }
        }

        // 3. 시각적 연출 ("쾅!" 하고 커지면서 서서히 투명해지기)
        Vector3 startScale = Vector3.zero; // 점(0)에서 시작
        Vector3 targetScale = new Vector3(finalRadius * 2f, finalRadius * 2f, 1f); // 지름(반경의 2배)까지 커짐

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color startColor = sr != null ? sr.color : Color.white;
        float time = 0f;

        while (time < expandDuration)
        {
            time += Time.deltaTime;
            float progress = time / expandDuration; // 0.0 에서 1.0 까지 증가

            // 크기는 순식간에 커지고 (Lerp)
            transform.localScale = Vector3.Lerp(startScale, targetScale, progress);

            
            yield return null; // 다음 프레임까지 대기
        }

        // 4. 폭발 연출이 끝나면 깔끔하게 파괴!
        Destroy(gameObject);
    }

    // 💡 개발용: 유니티 씬(Scene) 창에서 폭발 반경을 빨간 원으로 미리 볼 수 있습니다.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, baseRadius);
    }
}