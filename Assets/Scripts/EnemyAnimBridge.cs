using UnityEngine;

public class EnemyAnimBridge : MonoBehaviour
{
    // 애니메이션 이벤트가 이 함수를 부르면, 부모한테 가서 찐 함수를 실행시킴!
    public void ExecuteAttackDamage()
    {
        GetComponentInParent<EnemyController>().ExecuteAttackDamage();
    }
}