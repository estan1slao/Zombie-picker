using UnityEngine;

public class VolumeLoader : MonoBehaviour
{
    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        AudioListener.volume = savedVolume;
    }
}