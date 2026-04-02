using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public float spawnRadius = 8.0f;

    [Header("시간 및 스폰 설정")]
    public float maxGameTime = 60f;
    public float startSpawnDelay = 1.0f;
    public float minSpwanDelay = 0.1f;

    float timer = 0f;
    float totalGameTime = 0f;
    private void Update()
    {
        timer += Time.deltaTime;
        totalGameTime += Time.deltaTime;
        float progress = Mathf.Clamp01(totalGameTime / maxGameTime);
        float currentSpawnDelay = Mathf.Lerp(startSpawnDelay, minSpwanDelay, progress);
        int spwanCount = 1 + Mathf.FloorToInt(progress * 4);
        if (timer > currentSpawnDelay)
        {
            for (int i = 0; i < spwanCount; i++)
            {
                SpawnSingleEnemy();
            }
            timer = 0f;
        }
    }
    private void SpawnSingleEnemy()
    {
        Vector2 spawnPos = (Vector2)player.position + Random.insideUnitCircle.normalized * spawnRadius;

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        EnemyController enemyScript = newEnemy.GetComponent<EnemyController>();
        if (enemyScript != null)
        {
            enemyScript.hp = 3 + Mathf.FloorToInt(totalGameTime / 10f);
        }
    }
}
