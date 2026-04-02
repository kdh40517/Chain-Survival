using UnityEngine;

public class ExpGem : MagneticItem
{
    public int expAmount = 10;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            GameManager.instance.AddExp(expAmount);
            Destroy(gameObject);
        }
        else if (collision.name == "MagnetArea")
        {
            StartAttraction(collision.transform.parent);
        }
    }
}
