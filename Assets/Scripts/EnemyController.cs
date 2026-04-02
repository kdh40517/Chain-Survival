using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2.0f;
    Transform player;
    public int hp = 3;
    public float knockbackPower = 10.0f;
    public bool isKnockBack = false;
    public bool isDead = false;
    public GameObject expGemPrefab;
    public GameObject magnetItemPrefab;
    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }
    private void Update()
    {
        if (player == null)
        {
            return;
        }
        if (!isKnockBack)
        {
            Vector2 dir = player.position - transform.position;
            transform.Translate(dir.normalized * speed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "MeleeAttack")
        {
            TakeDamage(1, player.position, 10f);

            ChainManager chain = player.GetComponent<ChainManager>();
            if (chain != null) chain.StartChainReaction(transform.position);
        }
    }
    public void TakeDamage(int damage, Vector2 hitPosition, float customKnockback)
    {
        if (isDead) return;
        hp -= damage;
        if (customKnockback > 0f)
        {
            Vector2 knockbackDir = ((Vector2)transform.position - hitPosition).normalized;
            StartCoroutine(KnockbackRoutine(knockbackDir, customKnockback));
        }

        if (hp <= 0)
        {
            GameManager.instance.AddCombo();
            float dropDice = Random.Range(0f, 100f);
            if (dropDice <= 30 && magnetItemPrefab != null)
            {
                Vector3 magnetPos = transform.position + new Vector3(0.5f, 0.5f, 0.5f);
                Instantiate(magnetItemPrefab, magnetPos, Quaternion.identity);
            }
            Die();
        }
    }
    private void Die()
    {
        isDead = true;
        Instantiate(expGemPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    IEnumerator KnockbackRoutine(Vector2 dir, float power)
    {
        isKnockBack = true;
        float timer = 0f;
        float duration = 0.15f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.Translate(dir * power * Time.deltaTime);
            yield return null;
        }
        isKnockBack = false;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            PlayerController playerScript = collision.gameObject.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(10);
            }
        }
    }
}
