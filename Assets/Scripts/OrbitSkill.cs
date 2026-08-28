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

    private Transform playerTransform;

    private void Start()
    {
        startTime = Time.time;

        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            transform.SetParent(null);
        }

        int myBonusDamage = 0;
        float myBonusSize = 0f;
        int extraSwords = 0;
        float bonusRadius = 0f;

        if (GameManager.instance.specificBonusDamage.ContainsKey(skillId))
        {
            myBonusDamage += GameManager.instance.specificBonusDamage[skillId];

            myBonusSize += GameManager.instance.specificBonusSize[skillId];

            extraSwords += GameManager.instance.specificBonusSwords[skillId];
            bonusRadius += GameManager.instance.specificBonusRadius[skillId];
            duration += GameManager.instance.specificBonusAngle[skillId];
        }

        skillDamage += myBonusDamage;
        finalRadius = Mathf.Clamp(baseRadius + bonusRadius, minRadius, maxRadius);
        int totalSwords = chainLevel + extraSwords;

        SpawnSwords(totalSwords, myBonusSize);

        Destroy(gameObject, duration);
    }

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

            Vector3 originalScale = swordVisualPrefab.transform.localScale;
            float visualScaleMult = 1.0f + (chainLevel - 1) * 0.2f + bonusSize;

            swordVisual.transform.localScale = originalScale * visualScaleMult;

            spawnedSwords.Add(swordVisual.transform);
        }
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            transform.position = playerTransform.position;
        }

        float progress = (Time.time - startTime) / duration;
        transform.localRotation = Quaternion.Euler(0f, 0f, progress * -360f);
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