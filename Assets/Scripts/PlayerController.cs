using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("콤보 타이밍 설정")]
    public float comboLinkDelay = 0.15f;
    public float comboWindow = 0.8f;
    public float fullRecoveryTime = 0.6f;

    public float speed = 5.0f;
    public GameObject attackArea;
    public float dashSpeed = 15.0f;
    public float dashDuration = 0.2f;

    private float lastAttackTime = -99f;
    private int comboStep = 0;
    private Animator anim;
    private SpriteRenderer sr;
    private bool isDashing = false;
    private Vector2 lastMoveDir = new Vector2(1f, 0f);
    SpriteRenderer attackEffectRenderer;

    public float attackCooldown = 0.5f;
    private float currentCooldown = 0f;
    public int maxHp = 100;
    public int currenHp;
    public Slider hpBar;

    public float invincibleTime = 0.5f;
    private bool isInvincible = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        attackEffectRenderer = attackArea.GetComponentInChildren<SpriteRenderer>();
        currenHp = maxHp;
        if (hpBar != null)
        {
            hpBar.maxValue = maxHp;
            hpBar.value = currenHp;
        }
    }
    void Update()
    {
        if (isDashing)
        {
            return;
        }
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);
        if (dir.x != 0 || dir.y != 0)
        {
            lastMoveDir = dir.normalized;
        }
        transform.Translate(dir.normalized * speed * Time.deltaTime);

        if (dir.magnitude > 0) 
            anim.SetBool("isRunning", true);
        else 
            anim.SetBool("isRunning", false);
        Vector3 playerScale = transform.localScale;
        if (x < 0)
        {
            // 왼쪽을 보면 X 크기를 마이너스로 만듭니다 (좌우 반전)
            playerScale.x = -Mathf.Abs(playerScale.x);
        }
        else if (x > 0)
        {
            // 오른쪽을 보면 X 크기를 플러스로 만듭니다 (원상 복구)
            playerScale.x = Mathf.Abs(playerScale.x);
        }
        transform.localScale = playerScale;
        currentCooldown += Time.deltaTime;
        float timeSinceLastAttack = Time.time - lastAttackTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 1. 일단 무조건 공격 못 한다고 막아둡니다. (엄격한 검문소)
            bool canAttack = false;

            // 2. 0타(처음 때림)일 때: 쿨타임(0.6초)이 지났으면 통과!
            if (comboStep == 0 && timeSinceLastAttack >= fullRecoveryTime)
            {
                canAttack = true;
            }
            // 3. 1타를 친 직후일 때: 
            else if (comboStep == 1)
            {
                // 다다닥! (최소 연결 시간은 지났고, 유예 시간은 안 지났을 때 통과!)
                if (timeSinceLastAttack >= comboLinkDelay && timeSinceLastAttack <= comboWindow)
                {
                    canAttack = true;
                }
                // 만약 너무 오래 멍때려서 콤보 시간을 놓쳤다면?
                else if (timeSinceLastAttack > comboWindow)
                {
                    comboStep = 0; // 콤보 초기화! 다시 1타부터 칠 준비를 합니다.
                    if (timeSinceLastAttack >= fullRecoveryTime)
                    {
                        canAttack = true; // 초기화된 김에 쿨타임 차 있으면 바로 1타 발사!
                    }
                }
            }

            // 4. 위의 검문소를 통과한 사람(true)만 공격을 실행합니다!
            if (canAttack)
            {
                PerformAttack();
            }
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            anim.SetTrigger("Dash");
            StartCoroutine(DashRoutine());
        }
    }
    void PerformAttack()
    {
        comboStep++;
        lastAttackTime = Time.time;

        anim.SetTrigger("Attack");
        StartCoroutine(AttackTimer());
        if (comboStep >= 2) comboStep = 0;
    }
    IEnumerator DashRoutine()
    {
        isDashing = true;
        float timer = 0f;
        while (timer < dashDuration)
        {
            timer += Time.deltaTime;
            transform.Translate(lastMoveDir * dashSpeed * 0.01f);
            yield return null;
        }
        isDashing = false;
    }
    IEnumerator AttackTimer()
    {
        // 👇 [핵심 1. 선딜레이] 애니메이션에서 칼을 치켜드는 시간만큼 잠깐 기다려줍니다!
        // (애니메이션 속도에 맞춰 이 숫자를 0.1f ~ 0.2f 사이로 조절해 보세요)
        yield return new WaitForSeconds(0.15f);

        // 👇 [핵심 2. 타격 발생] 이제 진짜 칼을 휘두르는 타이밍! 공격 판정을 켭니다.
        attackArea.SetActive(true);

        // (테스트용으로 점점 커지던 코드는 지우고, 원하는 고정 크기로 둡니다. 필요시 1.5f 등으로 수정)
        attackArea.transform.localScale = new Vector2(1f, 1f);

        // 이펙트 색상 불투명하게(보이게) 설정
        Color effColor = attackEffectRenderer.color;
        effColor.a = 1f;
        attackEffectRenderer.color = effColor;

        // 👇 [핵심 3. 판정 유지] 적이 맞을 수 있도록 아주 짧은 시간(칼을 뻗고 있는 시간)만 판정을 유지합니다.
        yield return new WaitForSeconds(0.1f);

        // 👇 [핵심 4. 타격 종료] 칼을 다 휘둘렀으니 공격 판정을 끕니다. (이후 애니메이션은 자연스럽게 Idle로 돌아감)
        attackArea.SetActive(false);
    }
    public void TakeDamage(int damage)
    {
        if (isInvincible || isDashing) return;
        int finalDamege = Mathf.Max(0, damage - GameManager.instance.globalBonusDefense);
        currenHp -= finalDamege;
        if (hpBar != null) hpBar.value = currenHp;
        if (currenHp <= 0)
        {
            Debug.Log("사망");
            GameManager.instance.GameOver();
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(InvincibleRoutine());
        }
    }
    IEnumerator InvincibleRoutine()
    {
        isInvincible = true;
        SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        mySprite.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(invincibleTime);
        mySprite.color = new Color(1, 1, 1, 1f);
        isInvincible = false;
    }
    public void UpdateMaxHp()
    {
        int newMaxHp = maxHp + GameManager.instance.globalBonusMaxHP;
        if (hpBar != null)
        {
            int hpDifference = newMaxHp - (int)hpBar.maxValue;
            currenHp += hpDifference;
            hpBar.maxValue = newMaxHp;
            hpBar.value = currenHp;
        }
    }
    public void UpdateMagneticRange()
    {
        Transform magnetArea = transform.Find("MagnetArea");
        if (magnetArea != null)
        {
            CircleCollider2D magnetCollider = magnetArea.GetComponent<CircleCollider2D>();
            if (magnetCollider != null)
            {
                float baseRadius = 3.0f;
                magnetCollider.radius = baseRadius + GameManager.instance.globalBonusMagneticRange;
            }
        }
    }
}
