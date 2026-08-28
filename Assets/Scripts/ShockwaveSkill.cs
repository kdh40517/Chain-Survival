using UnityEngine;
using System.Collections;

public class ShockwaveSkill : SkillEffect
{
    [Header("충격파 고유 설정")]
    public float baseRadius = 3.0f;

    private void Start()
    {
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
        int myBonusDamage = 0;
        float myBonusSize = 0f;

        if (GameManager.instance.specificBonusDamage.ContainsKey(skillId))
        {
            myBonusDamage += GameManager.instance.specificBonusDamage[skillId];
            myBonusSize += GameManager.instance.specificBonusSize[skillId];
        }

        skillDamage += myBonusDamage;

        float finalRadius = baseRadius + (chainLevel * 0.5f) + myBonusSize;
        float finalKnockback = knockbackPower * (1f + chainLevel * 0.2f);

        if (chainLevel >= 6)
        {
            finalRadius *= 3.0f;
            skillDamage *= 2;
            finalKnockback *= 2.0f;
            Debug.Log("[Shockwave] 6체인 - 범위 3배, 피해 2배 적용");
        }

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

        Vector3 targetScale = new Vector3(finalRadius * 2f, finalRadius * 2f, 1f);
        transform.localScale = targetScale;

        yield return new WaitForSeconds(0.2f);

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, baseRadius);
    }
}