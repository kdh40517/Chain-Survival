using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningSkill : SkillEffect
{
    [Header("번개 고유 설정")]
    public int baseBounces = 3;
    public float bounceRadius = 5f;
    public float displayDuration = 0.3f;

    [Header("6체인 특별 설정")]
    public float splashRadius = 2.5f; // 감전 폭발 범위

    [Header("애니메이션 설정")]
    public int columns = 4;
    public int rows = 3;
    public float framesPerSecond = 20f;

    private LineRenderer lr;
    private HashSet<EnemyController> hitEnemies = new HashSet<EnemyController>();

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        if (lr == null) Debug.Log("🚨 LineRenderer 컴포넌트가 필요합니다!");
        StartCoroutine(AnimateSpriteSheet());

        float myBonusSize = 0f;
        int myBonusChains = 0;

        if (GameManager.instance.specificBonusDamage.ContainsKey(skillId))
        {
            skillDamage += GameManager.instance.specificBonusDamage[skillId];
            myBonusSize += GameManager.instance.specificBonusSize[skillId];
            myBonusChains += GameManager.instance.specificBonusChains[skillId];
        }

        float finalBounceRadius = bounceRadius + myBonusSize;
        int maxBounces = baseBounces + (chainLevel - 1) + myBonusChains;
        StartCoroutine(FireLightning(maxBounces, finalBounceRadius));
    }

    IEnumerator AnimateSpriteSheet()
    {
        Vector2 size = new Vector2(1f / columns, 1f / rows);
        if (lr == null || lr.material == null) yield break;
        lr.material.mainTextureScale = size;
        int totalFrames = columns * rows;
        int currentFrame = 0;

        while (true)
        {
            int uIndex = currentFrame % columns;
            int vIndex = currentFrame / columns;
            float offsetX = uIndex * size.x;
            float offsetY = (1.0f - size.y) - (vIndex * size.y);

            lr.material.mainTextureOffset = new Vector2(offsetX, offsetY);
            currentFrame = (currentFrame + 1) % totalFrames;
            yield return new WaitForSeconds(1f / framesPerSecond);
        }
    }

    IEnumerator FireLightning(int maxBounces, float radius)
    {
        EnemyController currentTarget = FindNearestEnemy(transform.position, radius);
        if (currentTarget == null)
        {
            Destroy(gameObject);
            yield break;
        }

        List<Vector3> linePoints = new List<Vector3>();
        linePoints.Add(transform.position);
        int bounceCount = 0;

        while (currentTarget != null && bounceCount < maxBounces)
        {
            // 타겟에게 데미지
            currentTarget.TakeDamage(skillDamage, currentTarget.transform.position, knockbackPower);
            hitEnemies.Add(currentTarget);

            linePoints.Add(currentTarget.transform.position);
            lr.positionCount = linePoints.Count;
            lr.SetPositions(linePoints.ToArray());

            // 💡 [6체인 효과] 번개가 맞은 곳에서 폭발!
            if (chainLevel >= 6)
            {
                ApplySplashDamage(currentTarget.transform.position, splashRadius);
            }

            bounceCount++;
            Vector3 lastPos = currentTarget.transform.position;
            yield return new WaitForSeconds(0.05f);

            currentTarget = FindNearestEnemy(lastPos, radius);
        }

        yield return new WaitForSeconds(displayDuration);
        if (lr != null) lr.positionCount = 0;
        Destroy(gameObject);
    }

    // 주변 적들에게 폭발 데미지를 주는 함수
    private void ApplySplashDamage(Vector3 center, float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, radius);
        foreach (Collider2D hit in colliders)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            // 이미 메인 번개에 맞은 적이 아니라면 튕김 데미지의 50%를 줍니다.
            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                enemy.TakeDamage(skillDamage / 2, center, knockbackPower * 0.5f);
            }
        }
    }

    private EnemyController FindNearestEnemy(Vector3 center, float maxRadius)
    {
        EnemyController[] allEnemies = FindObjectsByType<EnemyController>(FindObjectsInactive.Exclude);
        EnemyController nearest = null;
        float minDistance = maxRadius;

        foreach (EnemyController enemy in allEnemies)
        {
            if (enemy == null || hitEnemies.Contains(enemy)) continue;
            float dist = Vector2.Distance(center, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = enemy;
            }
        }
        return nearest;
    }
}