using UnityEngine;
using System.Collections;

public class ShockwaveSkill : SkillEffect
{
    [Header("충격파 고유 설정")]
    public float baseRadius = 3.0f;     // 기본 폭발 반경
    // 서서히 커지는 속도(expandDuration) 변수는 쳐냈습니다!

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
        float finalKnockback = knockbackPower * (1f + chainLevel * 0.2f);

        // 🚨 [대망의 6체인 궁극기 로직] 🚨
        if (chainLevel >= 6)
        {
            finalRadius *= 3.0f; // 범위 3배!! (화면 전체 폭발)
            skillDamage *= 2;    // 데미지도 2배!
            finalKnockback *= 2.0f;
            Debug.Log("💥 [충격파 6체인] 화면 붕괴 붕괴!! 싹 다 밀쳐냄!!");
        }

        // 2. 데미지 즉시 판정 (폭발 범위 안의 모든 적을 한 번에 긁어와서 때립니다.)
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

        // 3. 시각적 연출 (스멀스멀 커지는 거 삭제! 1프레임 만에 즉시 쾅!)
        Vector3 targetScale = new Vector3(finalRadius * 2f, finalRadius * 2f, 1f);
        transform.localScale = targetScale; // 목표 크기로 한 번에 키움

        // 💡 1프레임 만에 파괴되면 유저 눈에 안 보이니, 터진 그림을 0.2초만 보여주고 지웁니다.
        // (만약 터지는 애니메이션 클립을 만들어 두셨다면 애니메이션 길이로 맞추셔도 됩니다)
        yield return new WaitForSeconds(0.2f);

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