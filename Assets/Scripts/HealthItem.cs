using UnityEngine;

public class HealItem : MonoBehaviour
{
    [Header("회복 설정")]
    public int healAmount = 20;

    private Transform targetPlayer;
    private float magnetSpeed = 5f;

    private void Update()
    {
        if (targetPlayer != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, magnetSpeed * Time.deltaTime);
            magnetSpeed += 15f * Time.deltaTime;
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
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Heal(healAmount);
            }

            Destroy(gameObject);
        }
    }
}