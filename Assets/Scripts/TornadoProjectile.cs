using UnityEngine;
using System.Collections.Generic;

public class TornadoProjectile : MonoBehaviour
{
    private int damage;
    private float knockback;
    private float speed;
    private float tickRate;
    private Vector3 moveDirection;

    private Dictionary<EnemyController, float> lastHitTimes = new Dictionary<EnemyController, float>();

    public void Setup(int dmg, float knock, float spd, float tick, Vector3 dir)
    {
        damage = dmg;
        knockback = knock;
        speed = spd;
        tickRate = tick;
        moveDirection = dir.normalized;
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                if (!lastHitTimes.ContainsKey(enemy) || Time.time - lastHitTimes[enemy] >= tickRate)
                {
                    enemy.TakeDamage(damage, transform.position, knockback);
                    lastHitTimes[enemy] = Time.time;
                }
            }
        }
    }
}