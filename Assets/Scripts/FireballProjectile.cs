using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public GameObject explosionEffect;

    private int damage;
    private float knockback;
    private float explosionRadius;
    private float speed = 10f;

    public void Setup(int dmg, float knock, float expRadius, float spd)
    {
        damage = dmg;
        knockback = knock;
        explosionRadius = expRadius;
        speed = spd;
    }

    private void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Explode();
        }
    }

    private void Explode()
    {
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
                GameObject fx = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                float effectScale = explosionRadius * 2.0f;
                fx.transform.localScale = new Vector3(effectScale, effectScale, 1f);
            }

            Destroy(gameObject);
        }

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
