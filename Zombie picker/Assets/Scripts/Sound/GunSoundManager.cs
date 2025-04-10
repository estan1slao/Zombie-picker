using UnityEngine;

public class GunSoundManager : MonoBehaviour
{
    public static GunSoundManager Instance;

    private float lastPlayTime;
    public float minInterval = 0.05f; // минимальный интервал между звуками
    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayShootSound(AudioClip clip)
    {
        if (Time.time - lastPlayTime > minInterval)
        {
            audioSource.PlayOneShot(clip);
            lastPlayTime = Time.time;
        }
    }
}