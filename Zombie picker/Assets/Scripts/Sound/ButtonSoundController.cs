using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ButtonSoundController : MonoBehaviour
{
    public AudioSource audioSource;
    public static ButtonSoundController Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
        
        SceneManager.activeSceneChanged += (current, next) => FindButtons();
    }

    private void Start() => FindButtons();

    private void FindButtons()
    {
        var buttons = FindObjectsOfType<Button>(true);
        foreach (var button in buttons)
        {
            button.onClick.AddListener(PlaySound);
        }
    }
    
    private void PlaySound()
    {
        audioSource.Play();
    }
}