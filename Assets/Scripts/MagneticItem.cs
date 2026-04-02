using UnityEngine;

public class MagneticItem : MonoBehaviour
{
    protected bool isAttracted = false;
    protected Transform playerTransform;

    protected float moveSpeed = 0f;
    public float acceleration = 15f;
    protected void Update()
    {
        if (isAttracted && playerTransform != null)
        {
            moveSpeed += acceleration * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
        }
    }
    public void StartAttraction(Transform player)
    {
        isAttracted = true;
        playerTransform = player;
    }
}
