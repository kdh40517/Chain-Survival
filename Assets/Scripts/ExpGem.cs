using UnityEngine;

public class ExpGem : MonoBehaviour
{
    [Header("기본 설정")]
    public int baseExpAmount = 10;

    [Header("보석 진화 설정 (시간 비례)")]
    public Color stage1Color = Color.green;
    public Color stage2Color = Color.blue;
    public Color stage3Color = Color.red;

    private int finalExpAmount;
    private SpriteRenderer sr;

    private Transform targetPlayer;
    private float magnetSpeed = 5f;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        CalculateGemPower();
    }

    private void Update()
    {
        if (targetPlayer != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, magnetSpeed * Time.deltaTime);
            magnetSpeed += 15f * Time.deltaTime;
        }
    }

    private void CalculateGemPower()
    {
        float gameTime = Time.timeSinceLevelLoad;

        if (gameTime < 60f)
        {
            finalExpAmount = baseExpAmount;
            if (sr != null) sr.color = stage1Color;
        }
        else if (gameTime < 180f)
        {
            finalExpAmount = baseExpAmount * 3;
            if (sr != null) sr.color = stage2Color;
        }
        else
        {
            finalExpAmount = baseExpAmount * 6;
            if (sr != null) sr.color = stage3Color;
        }
    }

    public void StartAttraction(Transform playerTransform)
    {
        targetPlayer = playerTransform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.AddExp(finalExpAmount);
            Destroy(gameObject);
        }
    }
}