using UnityEngine;

public class MagnetItem : MagneticItem
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            ExpGem[] allGems = FindObjectsOfType<ExpGem>();
            foreach (ExpGem gem in allGems)
            {
                gem.StartAttraction(collision.transform);
            }
            Destroy(gameObject);
        }
        else if (collision.name == "MagnetArea")
        {
            StartAttraction(collision.transform.parent);
        }
    }
}
