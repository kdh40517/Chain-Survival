using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    // 💡 애니메이션이 몇 초 동안 나오는지 인스펙터에 적어주세요! (예: 12장에 0.5초)
    public float delay;
    public AudioSource myAudio;
    private static float lastSoundTime = 0f;

    private void Start()
    {
        // 스피커가 연결되어 있다면 쿨타임 체크!
        if (myAudio != null)
        {
            // 0.1초가 지났을 때만 진짜로 소리를 낸다
            if (Time.time - lastSoundTime > 0.1f)
            {
                myAudio.Play();
                lastSoundTime = Time.time;
            }
            else
            {
                // 0.1초 안에 동시에 터지는 나머지 폭발들은 강제 묵비권(음소거)!
                myAudio.mute = true;
            }
        }
        // delay초 뒤에 나(폭발 이펙트)를 파괴하라!
        Destroy(gameObject, delay);
    }
}