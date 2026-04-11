using UnityEngine;

public class MagnetItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. 맵에 있는 모든 경험치 보석 당기기 (최신 문법으로 수정)
            ExpGem[] gems = Object.FindObjectsByType<ExpGem>(FindObjectsInactive.Exclude);
            foreach (ExpGem gem in gems)
            {
                gem.StartAttraction(collision.transform);
            }

            // 2. 맵에 있는 모든 힐템 당기기 (최신 문법으로 수정)
            HealItem[] potions = Object.FindObjectsByType<HealItem>(FindObjectsInactive.Exclude);
            foreach (HealItem potion in potions)
            {
                potion.StartAttraction(collision.transform);
            }

            // 자석 파괴
            Destroy(gameObject);
        }
    }
}