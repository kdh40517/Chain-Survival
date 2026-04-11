using UnityEngine;

public class PlayerMagnetArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 자석 범위에 '경험치 보석'이 닿으면 플레이어 쪽으로 당기기
        ExpGem gem = collision.GetComponent<ExpGem>();
        if (gem != null)
        {
            // transform.parent는 MagnetArea의 부모인 'Player' 본체를 뜻합니다.
            gem.StartAttraction(transform.parent);
        }

        // 2. 자석 범위에 '힐템'이 닿으면 플레이어 쪽으로 당기기
        HealItem potion = collision.GetComponent<HealItem>();
        if (potion != null)
        {
            potion.StartAttraction(transform.parent);
        }
    }
}