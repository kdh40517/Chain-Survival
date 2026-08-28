using UnityEngine;

public class Reposition : MonoBehaviour
{
    [Header("설정")]
    public Transform player;

    private BoxCollider2D coll;

    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (player == null) return;
        float sizeX = coll.bounds.size.x;
        float sizeY = coll.bounds.size.y;

        float diffX = player.position.x - transform.position.x;
        float diffY = player.position.y - transform.position.y;

        float dirX = diffX < 0 ? -1 : 1;
        float dirY = diffY < 0 ? -1 : 1;

        if (Mathf.Abs(diffX) > sizeX * 1.5f)
        {
            transform.Translate(Vector3.right * dirX * sizeX * 3f);
        }

        if (Mathf.Abs(diffY) > sizeY * 1.5f)
        {
            transform.Translate(Vector3.up * dirY * sizeY * 3f);
        }
    }
}