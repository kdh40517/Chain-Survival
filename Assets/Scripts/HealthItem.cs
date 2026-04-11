using UnityEngine;

public class HealItem : MonoBehaviour
{
    [Header("회복 설정")]
    public int healAmount = 20; // 회복할 체력 양

    // 자석에 끌려가는 효과용
    private Transform targetPlayer;
    private float magnetSpeed = 5f;

    private void Update()
    {
        // 자석 아이템을 먹어서 타겟이 설정되면 플레이어 쪽으로 날아갑니다.
        if (targetPlayer != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, magnetSpeed * Time.deltaTime);
            magnetSpeed += 15f * Time.deltaTime;
        }
    }

    // 자석 아이템이 호출할 함수
    public void StartAttraction(Transform playerTransform)
    {
        targetPlayer = playerTransform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                // 플레이어의 체력을 회복시키는 함수 호출!
                player.Heal(healAmount);
            }

            // 먹었으니 아이템 삭제
            Destroy(gameObject);
        }
    }
}