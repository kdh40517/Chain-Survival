using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    // 💡 애니메이션이 몇 초 동안 나오는지 인스펙터에 적어주세요! (예: 12장에 0.5초)
    public float delay;

    private void Start()
    {
        // delay초 뒤에 나(폭발 이펙트)를 파괴하라!
        Destroy(gameObject, delay);
    }
}