﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using YG;
using Random = UnityEngine.Random;


public class CloneController : MonoBehaviour
{
    [Header("Settings")] 
    public GameObject clonePrefab;
    public float cloneSpacing = 1.5f;
    public float roadWidth = 10f;
    public float distanceFromCamera = 10f;
    public float fixedY = 50f;
    
    [Header("HP")]
    public Slider hpSlider;
    
    [Header("Icon")]
    public Image iconImage;
    public TextMeshProUGUI dpsText;
    
    [Header("Other")]
    public Pause pause;
    public GameObject loseCanvas;
    public Button adButton;
    public GameObject pauseCanvas;
    
    private readonly List<Clone> activeClones = new();
    
    private bool isMouseHolding;
    private Camera mainCamera;
    private Vector3 targetPosition;
    
    private int clonesToSpawnAfterAd;

    public Clone GetNearestToMouseClone() => 
        activeClones.OrderBy(c => Vector3.Distance(c.transform.position, targetPosition)).FirstOrDefault();

    public void SpawnClones(int count)
    {
        int availableSlots = 75 - activeClones.Count;
        int spawnCount = Mathf.Min(count, availableSlots);

        for (var i = 0; i < spawnCount; i++)
            SpawnClone();

        Debug.Log($"spawned {spawnCount} (requested: {count})");
    }
    
    public void DestroyClones(int count)
    {
        if (count > activeClones.Count)
            count = activeClones.Count;
        
        activeClones.RemoveAll(clone => clone == null);
        
        for (var i = 0; i < count; i++)
            activeClones[i].TakeDamage(999999999f);
        
        activeClones.RemoveAll(clone => clone == null);
        
        UpdateTotalHealth();
        
        Debug.Log($"destroyed {count}");
    }
    
    private void Start()
    {
        mainCamera = Camera.main;
        SpawnClone();
    }

    public void OnHold(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) return;
        isMouseHolding = context.performed;
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isMouseHolding) return;
        
        var mousePosition = context.ReadValue<Vector2>();
        var distance = Mathf.Abs(mainCamera.transform.position.y) + distanceFromCamera;
        var screenPos = new Vector3(mousePosition.x, mousePosition.y, distance);
        
        var worldPosition = mainCamera.ScreenToWorldPoint(screenPos);
        
        var clampedZ = Mathf.Clamp(worldPosition.z, -roadWidth / 2, roadWidth / 2);
        
        fixedY = activeClones.Count;
                
        targetPosition = new Vector3(worldPosition.x, fixedY, clampedZ);
    }
    
    private void Update()
    {
        if (isMouseHolding)
        {
            MoveClones();
        }
    }
    
    private void SpawnClone()
    {
        var position = Vector3.zero;
        
        if (activeClones.Count > 1)
        {
            var randomClonePosition = activeClones[Random.Range(0, activeClones.Count)].transform.position;
            position = new Vector3(
                randomClonePosition.x + Random.Range(-cloneSpacing, cloneSpacing),
                randomClonePosition.y,
                randomClonePosition.z + Random.Range(-cloneSpacing, cloneSpacing)
            );
        }
        
        var clone = Instantiate(clonePrefab, position, Quaternion.Euler(0, -90, 0)).GetComponent<Clone>();
        activeClones.Add(clone);
        
        clone.OnHealthChanged += UpdateTotalHealth;
        clone.OnHealTriggered += HealTriggered;
        clone.OnGunChangeTriggered += ChangeGuns;
        UpdateTotalHealth();
    }
    
    public void SpawnClonesForAd(int clonesCount)
    {
        YandexGame.RewVideoShow(1); // ID 1 должен совпадать с кейсом в OnRewarded
        clonesToSpawnAfterAd = clonesCount;
        pauseCanvas.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += OnRewarded;
    }

    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= OnRewarded;
    }

    private void OnRewarded(int id)
    {
        if (id == 1) // Это должен быть тот же ID, что и в RewVideoShow
        {
            for (int i = 0; i < clonesToSpawnAfterAd; i++)
            {
                SpawnClone();
            }

            UpdateTotalHealth();
            adButton.gameObject.SetActive(false);
            pauseCanvas.gameObject.SetActive(false);
        }
    }

    private void MoveClones()
    {
        activeClones.RemoveAll(clone => clone == null);

        foreach (var clone in activeClones)
        {
            if (clone != null)
            {
                clone.Follow(targetPosition, activeClones);
            }
        }
    }

    private void UpdateTotalHealth()
    {
        CheckLose();
        StartCoroutine(SmoothFill(GetTotalHealth(), GetMaxTotalHealth()));
    }
    
    private IEnumerator SmoothFill(float targetValue, float maxValueTarget)
    {
        var startValue = hpSlider.value;
        var maxValue = hpSlider.maxValue;
        var duration = 0.5f;
        var elapsed = 0f;
 
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            hpSlider.value = Mathf.Lerp(startValue, targetValue, elapsed / duration);
            hpSlider.maxValue = Mathf.Lerp(maxValue, maxValueTarget, elapsed / duration);
            yield return null;
        }
 
        hpSlider.value = targetValue;
        hpSlider.maxValue = maxValueTarget;
    }

    private void CheckLose()
    {
        activeClones.RemoveAll(clone => clone == null);
    
        if (activeClones.Count == 0 || GetTotalHealth() <= 0)
        {
            if (!loseCanvas.activeSelf)
            {
                pause.PauseGame();
                loseCanvas.SetActive(true);
            }
        }
    }

    private void HealTriggered()
    {
        activeClones.RemoveAll(clone => clone == null);
        
        foreach (var clone in activeClones)
        {
            clone.Heal();
        }
    }
    private void ChangeGuns(GunData gunData)
    {
        iconImage.sprite = gunData.iconImage.sprite;
        dpsText.text = $"DPS: {gunData.damage / (1 / gunData.fireRate)}";
        
        foreach (var clone in activeClones)
        {
            clone.ChangeWeapon(gunData);
        }
    }

    private float GetTotalHealth()
    {
        if (activeClones.Count == 0)
            return 0;
        return activeClones.Sum(clone => clone.CurrentHealth);
    }

    private float GetMaxTotalHealth()
    {
        if (activeClones.Count == 0)
             return 0;
        return activeClones.Sum(clone => clone.maxHealth);
    }
}