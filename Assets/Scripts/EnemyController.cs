using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("아이템 드랍 확률")]
    public GameObject magnetItemPrefab;
    public float magnetDropChance = 2.0f; // 자석 2%

    public GameObject healthItemPrefab;   // 💡 새로 추가: 체력 아이템 프리팹
    public float healthDropChance = 5.0f; // 💡 새로 추가: 체력 아이템 5% 확률

    [Header("이동 및 전투 스탯")]
    public float speed = 2.0f;
    public int hp = 3;
    public float knockbackPower = 10.0f;

    // 💡 새로 추가된 전투 관련 변수들
    public float attackRange = 1.5f;     // 공격 사거리 (이 안으로 오면 때림)
    public float attackCooldown = 2.0f;  // 연속 공격 방지용 쿨타임
    public int attackDamage = 10;        // 플레이어에게 줄 데미지
    private float lastAttackTime = 0f;   // 마지막으로 공격한 시간 기억

    [Header("연결")]
    public Animator anim;                // 💡 자식(Orc)의 애니메이터 연결칸!
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
        // 💡 플레이어가 없거나 이미 죽었으면 아무것도 안 함 (시체 움직임 방지)
        if (player == null || isDead) return;

        // 플레이어와의 거리 계산
        Vector2 dir = player.position - transform.position;
        float distance = dir.magnitude;

        // 💡 오크가 플레이어 방향을 바라보게 스프라이트 뒤집기 (선택)
        if (anim != null && dir.x != 0)
        {
            anim.transform.localScale = new Vector3(dir.x < 0 ? -1 : 1, 1, 1);
        }

        if (!isKnockBack)
        {
            if (distance <= attackRange)
            {
                // 💡 사거리 안: 걷기 애니메이션 끄고 제자리에 멈춤
                if (anim != null) anim.SetBool("isWalking", false);

                // 쿨타임이 지났다면 공격 발동!
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    AttackPlayer();
                }
            }
            else
            {
                // 💡 사거리 밖: 걷기 애니메이션 켜고 다가가기
                if (anim != null) anim.SetBool("isWalking", true);
                transform.Translate(dir.normalized * speed * Time.deltaTime);
            }
        }
    }

    private void AttackPlayer()
    {
        lastAttackTime = Time.time; // 공격 시간 갱신

        if (anim != null)
        {
            // 💡 0 또는 1을 주사위 굴려서 뽑은 다음 AttackIndex 스위치에 넣기
            anim.SetInteger("AttackIndex", Random.Range(0, 2));
            anim.SetTrigger("Attack"); // 공격 방아쇠 당기기!
        }
    }

    // 🚨 아주 중요: 이전 대화에서 배운 '애니메이션 이벤트'에 연결할 함수입니다!
    // 오크가 도끼를 내리찍는 순간에만 이 함수가 실행됩니다.
    public void ExecuteAttackDamage()
    {
        if (player == null || isDead) return;

        // 도끼 찍는 순간에 플레이어가 사거리 안에 있는지 한 번 더 확인
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange + 0.5f) // 약간의 판정 여유 (+0.5f)
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
        // 1. 이미 죽은 놈은 무시 (완벽 방어막)
        if (isDead) return;

        hp -= damage;

        if (hp <= 0)
        {
            // 💡 [죽었을 때]
            if (anim != null)
            {
                anim.ResetTrigger("Hurt"); // 혹시라도 예약된 피격 모션 취소
                // Die 방아쇠는 밑에 있는 Die() 함수 안에서 이미 당기고 있으니 여기서 또 당길 필요 없습니다!
            }
            Die();
        }
        else
        {
            // 💡 [살았을 때] 
            // 안 죽었을 때만 억! 하고 밀려나게 만듭니다.
            if (anim != null) anim.SetTrigger("Hurt");

            if (customKnockback > 0f)
            {
                Vector2 knockbackDir = ((Vector2)transform.position - hitPosition).normalized;
                StartCoroutine(KnockbackRoutine(knockbackDir, customKnockback));
            }
        }
        if (myAudio != null && hitSound != null)
        {
            // 💡 0.05초가 지났을 때만 소리를 내라! (초당 최대 20번 제한)
            if (Time.time - lastHitSoundTime > 0.05f)
            {
                myAudio.pitch = Random.Range(0.8f, 1.2f); // 타격음이 덜 질리게 음높이 랜덤 살짝!
                myAudio.PlayOneShot(hitSound, 0.7f);      // 소리 크기도 0.4(40%)로 살짝 줄임
                lastHitSoundTime = Time.time;             // 마지막으로 소리 낸 시간 리셋
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

        // 💡 주사위를 0~100 사이로 굴립니다.
        float dropDice = Random.Range(0f, 100f);

        // 1. 자석 당첨 (0 ~ 2)
        if (dropDice <= magnetDropChance && magnetItemPrefab != null)
        {
            Vector3 magnetPos = transform.position + new Vector3(0.5f, 0.5f, 0f);
            Instantiate(magnetItemPrefab, magnetPos, Quaternion.identity);
        }
        // 2. 체력 물약 당첨 (2 ~ 7 사이, 즉 5% 확률)
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