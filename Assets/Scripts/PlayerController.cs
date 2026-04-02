using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public GameObject attackArea;
    public float dashSpeed = 15.0f;
    public float dashDuration = 0.2f;

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

        currentCooldown += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && currentCooldown >= attackCooldown)
        {
            currentCooldown = 0f;
            attackArea.SetActive(true);
            StartCoroutine(AttackTimer());
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(DashRoutine());
        }
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
        float duration = 0.2f;
        float time = 0f;
        Color effColor = attackEffectRenderer.color;
        while (time < duration)
        {
            time += Time.deltaTime;
            float ratio = time / duration;
            attackArea.transform.localScale = Vector2.Lerp(new Vector2(1f, 1f), new Vector2(2f, 2f), ratio);
            effColor.a = Mathf.Lerp(1f, 0f, ratio);
            attackEffectRenderer.color = effColor;

            yield return null;
        }
        //attackArea.transform.localScale = new Vector2(1f, 1f);
        //effColor.a = 1f;
        //attackEffectRenderer.color = effColor;

        //attackArea.transform.localScale = new Vector2(2.0f, 2.0f);
        //effColor.a = 0f;
        //attackEffectRenderer.color = effColor;
        //yield return new WaitForSeconds(0.2f);
        attackArea.SetActive(false);
    }
    public void TakeDamage(int damage)
    {
        if (isInvincible || isDashing) return;
        currenHp -= damage;
        if (hpBar != null) hpBar.value = currenHp;
        if (currenHp <= 0)
        {
            Debug.Log("»ç¸Á");
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
}
