using UnityEngine;

public class EnemyAnimBridge : MonoBehaviour
{
    public void ExecuteAttackDamage()
    {
        GetComponentInParent<EnemyController>().ExecuteAttackDamage();
    }
}