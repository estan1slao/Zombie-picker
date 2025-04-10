using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AmbientSoundController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip menuSound;
    public AudioClip gameSound;

    public static AmbientSoundController Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.activeSceneChanged += (current, next) =>
        {
            audioSource.clip = next.name == "MainMenu" ? menuSound : gameSound;
            audioSource.Play();
        };
    }
}