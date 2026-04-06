using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public GameObject explosionEffect; // 나중에 터지는 이펙트 넣을 곳 (없어도 됨)

    private int damage;
    private float knockback;
    private float explosionRadius;
    private float speed = 10f; // 날아가는 속도

    // 발사대(Hub)가 불덩이를 쏠 때 스탯을 전달해주는 함수
    public void Setup(int dmg, float knock, float expRadius, float spd)
    {
        damage = dmg;
        knockback = knock;
        explosionRadius = expRadius;
        speed = spd;
    }

    private void Update()
    {
        // 오른쪽(앞)으로 계속 날아갑니다!
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 적과 부딪히면 쾅! 폭발합니다.
        if (collision.CompareTag("Enemy"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        // 1. 폭발 범위(explosionRadius) 안의 모든 적을 찾아서 광역 데미지!
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D col in hitEnemies)
        {
            EnemyController enemy = col.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, transform.position, knockback);
            }
            if (explosionEffect != null)
            {
                // Instantiate(원본, 위치, 회전);
                GameObject fx = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                float effectScale = explosionRadius * 2.0f;
                fx.transform.localScale = new Vector3(effectScale, effectScale, 1f);
            }

            // 3. 파이어볼 소멸!
            Destroy(gameObject);
        }

        // 2. 터지는 시각 효과 생성 (나중에 이펙트 프리팹 생기면 연결하세요!)
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // 3. 파이어볼 소멸!
        Destroy(gameObject);
    }

    // 💡 개발할 때 폭발 범위 눈으로 보려고 그리는 선 (플레이할 땐 안 보임)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
