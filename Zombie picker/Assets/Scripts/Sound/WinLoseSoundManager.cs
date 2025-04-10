using System;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class WinLoseSoundManager : MonoBehaviour
{
    public AudioClip sound;
    public AudioMixerGroup audioMixerGroup;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.outputAudioMixerGroup = audioMixerGroup;
    }

    private void OnEnable()
    {
        audioSource.PlayOneShot(sound);
    }
}
