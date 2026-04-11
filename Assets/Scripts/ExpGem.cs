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

    // 💡 자석 끌어당기기용 변수 추가
    private Transform targetPlayer;
    private float magnetSpeed = 5f;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        CalculateGemPower();
    }

    private void Update()
    {
        // 💡 자석 효과 발동 중: targetPlayer가 있다면 플레이어 쪽으로 날아가기!
        if (targetPlayer != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, magnetSpeed * Time.deltaTime);
            magnetSpeed += 15f * Time.deltaTime; // 닿을 때까지 속도가 점점 빨라집니다 (착! 붙는 타격감)
        }
    }

    private void CalculateGemPower()
    {
        // GameManager에서 게임이 시작된 지 얼마나 지났는지 확인
        float gameTime = Time.timeSinceLevelLoad;

        // 1분(60초) 지날 때마다 보석의 가치 상승
        if (gameTime < 60f)
        {
            finalExpAmount = baseExpAmount;
            if (sr != null) sr.color = stage1Color;
        }
        else if (gameTime < 180f)
        {
            finalExpAmount = baseExpAmount * 3; // 경험치 3배
            if (sr != null) sr.color = stage2Color;
        }
        else
        {
            finalExpAmount = baseExpAmount * 6; // 경험치 6배
            if (sr != null) sr.color = stage3Color;
        }
    }

    // 🚨 MagnetItem이 부르던 바로 그 함수! 다시 추가했습니다.
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