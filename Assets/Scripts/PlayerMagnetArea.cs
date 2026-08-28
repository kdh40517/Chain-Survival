using UnityEngine;

public class PlayerMagnetArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ExpGem gem = collision.GetComponent<ExpGem>();
        if (gem != null)
        {
            gem.StartAttraction(transform.parent);
        }

        HealItem potion = collision.GetComponent<HealItem>();
        if (potion != null)
        {
            potion.StartAttraction(transform.parent);
        }
    }
}