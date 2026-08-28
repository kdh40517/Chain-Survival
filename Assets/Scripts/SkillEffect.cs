using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    public string skillId;
    SpriteRenderer mySprite;
    public float targetSizeMult = 1f;
    public int skillDamage = 1;
    [HideInInspector] public int chainLevel = 1;
    [Header("스킬 발동 위치")]
    public bool spwnOnPlayer = false;
    [Header("스킬 고유 능력치")]
    public float knockbackPower = 0f;
    void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
        StartCoroutine(AnimateAndDestroy());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyController enemy = collision.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(skillDamage, transform.position, knockbackPower);
        }
    }
    IEnumerator AnimateAndDestroy()
    {
        float duration = 0.5f;
        float time = 0f;
        Color effColor = mySprite.color;
        while (time < duration)
        {
            time += Time.deltaTime;
            float ratio = time / duration;
            transform.localScale = Vector2.Lerp(new Vector2(0.1f, 0.1f), new Vector2(targetSizeMult, targetSizeMult), ratio);
            effColor.a = Mathf.Lerp(1f, 0f, ratio);
            mySprite.color = effColor;
            yield return null;
        }
        Destroy(gameObject);
    }
}
