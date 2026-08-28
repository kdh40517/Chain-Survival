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
            playerScale.x = -Mathf.Abs(playerScale.x);
        }
        else if (x > 0)
        {
            playerScale.x = Mathf.Abs(playerScale.x);
        }
        transform.localScale = playerScale;
        currentCooldown += Time.deltaTime;
        float timeSinceLastAttack = Time.time - lastAttackTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool canAttack = false;

            if (comboStep == 0 && timeSinceLastAttack >= fullRecoveryTime)
            {
                canAttack = true;
            }
            else if (comboStep == 1)
            {
                if (timeSinceLastAttack >= comboLinkDelay && timeSinceLastAttack <= comboWindow)
                {
                    canAttack = true;
                }
                else if (timeSinceLastAttack > comboWindow)
                {
                    comboStep = 0;
                    if (timeSinceLastAttack >= fullRecoveryTime)
                    {
                        canAttack = true;
                    }
                }
            }

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
        yield return new WaitForSeconds(0.15f);

        attackArea.SetActive(true);

        attackArea.transform.localScale = new Vector2(1f, 1f);

        Color effColor = attackEffectRenderer.color;
        effColor.a = 1f;
        attackEffectRenderer.color = effColor;

        yield return new WaitForSeconds(0.1f);

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
            Debug.Log("[Player] 사망");
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
    public void Heal(int amount)
    {
        if (currenHp >= maxHp) return;

        currenHp += amount;

        if (currenHp > maxHp)
        {
            currenHp = maxHp;
        }

        if (hpBar != null)
        {
            hpBar.value = currenHp;
        }

        Debug.Log($"[Player] 체력 회복 - {currenHp}/{maxHp}");
    }
}
