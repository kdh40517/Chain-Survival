using UnityEngine;

public class MagnetItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ExpGem[] gems = Object.FindObjectsByType<ExpGem>(FindObjectsInactive.Exclude);
            foreach (ExpGem gem in gems)
            {
                gem.StartAttraction(collision.transform);
            }

            HealItem[] potions = Object.FindObjectsByType<HealItem>(FindObjectsInactive.Exclude);
            foreach (HealItem potion in potions)
            {
                potion.StartAttraction(collision.transform);
            }

            Destroy(gameObject);
        }
    }
}