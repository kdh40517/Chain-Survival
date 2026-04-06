using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrbitSkill : SkillEffect
{
    [Header("공전 고유 설정")]
    public GameObject swordVisualPrefab;
    public float baseRadius = 2.5f;
    public float duration = 1.0f;

    public float minRadius = 1.5f;
    public float maxRadius = 5.0f;

    private float startTime;
    private List<Transform> spawnedSwords = new List<Transform>();
    private float finalRadius;

    // 👇 플레이어의 위치를 추적할 변수 추가
    private Transform playerTransform;

    private void Start()
    {
        startTime = Time.time;

        // 1. 플레이어를 찾아서 목표물로 삼고, '부모-자식' 관계는 끊어버립니다! (반전 버그 완벽 해결 ⭐)
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            transform.SetParent(null); // "난 이제 독립할게! 좌우 반전 영향 안 받을거야!"
        }

        int myBonusDamage = 0;
        float myBonusSize = 0f; // 💡 사이즈 보너스 변수 추가!
        int extraSwords = 0;
        float bonusRadius = 0f;

        if (GameManager.instance.specificBonusDamage.ContainsKey(skillId))
        {
            myBonusDamage += GameManager.instance.specificBonusDamage[skillId];

            // 💡 사이즈 보너스를 드디어 사물함에서 꺼내옵니다!
            myBonusSize += GameManager.instance.specificBonusSize[skillId];

            extraSwords += GameManager.instance.specificBonusSwords[skillId];
            bonusRadius += GameManager.instance.specificBonusRadius[skillId];
            duration += GameManager.instance.specificBonusAngle[skillId];
        }

        skillDamage += myBonusDamage;
        finalRadius = Mathf.Clamp(baseRadius + bonusRadius, minRadius, maxRadius);
        int totalSwords = chainLevel + extraSwords;

        // 💡 사이즈 보너스도 SpawnSwords 함수에 같이 넘겨줍니다.
        SpawnSwords(totalSwords, myBonusSize);

        Destroy(gameObject, duration);
    }

    // 💡 매개변수에 bonusSize 추가!
    private void SpawnSwords(int count, float bonusSize)
    {
        if (swordVisualPrefab == null) return;

        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float currentAngle = i * angleStep;
            float angleRad = currentAngle * Mathf.Deg2Rad;

            Vector3 spawnPos = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f) * finalRadius;

            GameObject swordVisual = Instantiate(swordVisualPrefab, transform);
            swordVisual.transform.localPosition = spawnPos;
            swordVisual.transform.localRotation = Quaternion.Euler(0f, 0f, currentAngle - 90f);

            // 💡 [사이즈 적용 핵심!] 기본 1 + 체인 증가량 + 카드에서 먹은 '특정 스킬 사이즈 보너스'
            Vector3 originalScale = swordVisualPrefab.transform.localScale;
            float visualScaleMult = 1.0f + (chainLevel - 1) * 0.2f + bonusSize; // 보너스 합체!

            swordVisual.transform.localScale = originalScale * visualScaleMult;

            spawnedSwords.Add(swordVisual.transform);
        }
    }

    private void Update()
    {
        // 💡 1. 플레이어 꽁무니 졸졸 따라다니기
        // 자식으로 들어가지 않아도, 매 프레임 위치를 똑같이 맞춰주면 완벽하게 따라갑니다.
        if (playerTransform != null)
        {
            transform.position = playerTransform.position;
        }

        // 💡 2. 회전 애니메이션
        float progress = (Time.time - startTime) / duration;
        transform.localRotation = Quaternion.Euler(0f, 0f, progress * -360f);
    }

    // --- 비비기 타격 로직 ---
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