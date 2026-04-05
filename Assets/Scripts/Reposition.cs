using UnityEngine;

public class Reposition : MonoBehaviour
{
    [Header("설정")]
    public Transform player;

    // 우리가 일일이 세지 않기 위해 유니티의 줄자(콜라이더)를 씁니다!
    private BoxCollider2D coll;

    void Start()
    {
        // 시작할 때 내 몸에 달린 줄자를 가져옵니다.
        coll = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (player == null) return;
        // 1. 줄자를 통해 이 맵 조각의 '가로 길이'와 '세로 길이'를 알아냅니다.
        float sizeX = coll.bounds.size.x;
        float sizeY = coll.bounds.size.y;

        // 2. 플레이어와 내(맵 조각) 거리 계산
        float diffX = player.position.x - transform.position.x;
        float diffY = player.position.y - transform.position.y;

        // 3. 방향(오른쪽으로 갔는지 왼쪽으로 갔는지) 구하기
        float dirX = diffX < 0 ? -1 : 1;
        float dirY = diffY < 0 ? -1 : 1;

        // [핵심!] 3x3 배열(총 9조각)일 때, 플레이어가 내 가로 길이의 1.5배 이상 멀어지면 화면 밖으로 나간 겁니다.
        if (Mathf.Abs(diffX) > sizeX * 1.5f)
        {
            // 맵 조각 3개 길이만큼 반대편으로 휙 던져줍니다!
            transform.Translate(Vector3.right * dirX * sizeX * 3f);
        }

        if (Mathf.Abs(diffY) > sizeY * 1.5f)
        {
            // 세로도 마찬가지!
            transform.Translate(Vector3.up * dirY * sizeY * 3f);
        }
    }
}