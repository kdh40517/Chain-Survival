using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("아이템 드랍 확률")]
    public GameObject magnetItemPrefab;
    public float magnetDropChance = 2.0f;

    public GameObject healthItemPrefab;
    public float healthDropChance = 5.0f;

    [Header("이동 및 전투 스탯")]
    public float speed = 2.0f;
    public int hp = 3;
    public float knockbackPower = 10.0f;

    public float attackRange = 1.5f;
    public float attackCooldown = 2.0f;
    public int attackDamage = 10;
    private float lastAttackTime = 0f;

    [Header("연결")]
    public Animator anim;
    public GameObject expGemPrefab;

    [Header("상태")]
    public bool isKnockBack = false;
    public bool isDead = false;

    Transform player;
    public AudioSource myAudio;
    public AudioClip hitSound;
    private static float lastHitSoundTime = 0f;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        lastAttackTime = -attackCooldown;
    }

    private void Update()
    {
        if (player == null || isDead) return;

        Vector2 dir = player.position - transform.position;
        float distance = dir.magnitude;

        if (anim != null && dir.x != 0)
        {
            anim.transform.localScale = new Vector3(dir.x < 0 ? -1 : 1, 1, 1);
        }

        if (!isKnockBack)
        {
            if (distance <= attackRange)
            {
                if (anim != null) anim.SetBool("isWalking", false);

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    AttackPlayer();
                }
            }
            else
            {
                if (anim != null) anim.SetBool("isWalking", true);
                transform.Translate(dir.normalized * speed * Time.deltaTime);
            }
        }
    }

    private void AttackPlayer()
    {
        lastAttackTime = Time.time;

        if (anim != null)
        {
            anim.SetInteger("AttackIndex", Random.Range(0, 2));
            anim.SetTrigger("Attack");
        }
    }

    public void ExecuteAttackDamage()
    {
        if (player == null || isDead) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange + 0.5f)
        {
            PlayerController playerScript = player.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(attackDamage);
            }
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

        if (hp <= 0)
        {
            if (anim != null)
            {
                anim.ResetTrigger("Hurt");
            }
            Die();
        }
        else
        {
            if (anim != null) anim.SetTrigger("Hurt");

            if (customKnockback > 0f)
            {
                Vector2 knockbackDir = ((Vector2)transform.position - hitPosition).normalized;
                StartCoroutine(KnockbackRoutine(knockbackDir, customKnockback));
            }
        }
        if (myAudio != null && hitSound != null)
        {
            if (Time.time - lastHitSoundTime > 0.05f)
            {
                myAudio.pitch = Random.Range(0.8f, 1.2f);
                myAudio.PlayOneShot(hitSound, 0.7f);
                lastHitSoundTime = Time.time;
            }
        }
    }

    private void Die()
    {
        isDead = true;
        if (anim != null) anim.SetTrigger("Die");
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        GameManager.instance.AddCombo();

        float dropDice = Random.Range(0f, 100f);

        if (dropDice <= magnetDropChance && magnetItemPrefab != null)
        {
            Vector3 magnetPos = transform.position + new Vector3(0.5f, 0.5f, 0f);
            Instantiate(magnetItemPrefab, magnetPos, Quaternion.identity);
        }
        else if (dropDice <= (magnetDropChance + healthDropChance) && healthItemPrefab != null)
        {
            Vector3 healthPos = transform.position + new Vector3(-0.5f, 0.5f, 0f);
            Instantiate(healthItemPrefab, healthPos, Quaternion.identity);
        }

        Instantiate(expGemPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject, 1.5f);
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
}