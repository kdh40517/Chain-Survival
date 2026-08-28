using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public float delay;
    public AudioSource myAudio;
    private static float lastSoundTime = 0f;

    private void Start()
    {
        if (myAudio != null)
        {
            if (Time.time - lastSoundTime > 0.1f)
            {
                myAudio.Play();
                lastSoundTime = Time.time;
            }
            else
            {
                myAudio.mute = true;
            }
        }
        Destroy(gameObject, delay);
    }
}