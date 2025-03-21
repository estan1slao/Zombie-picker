using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class CloneController : MonoBehaviour
{
    [Header("Settings")] 
    public GameObject clonePrefab;
    public float cloneSpacing = 1.5f;
    public float roadWidth = 10f;
    public float distanceFromCamera = 10f;
    
    [Header("Text")]
    public TextMeshProUGUI hpText;
    
    private readonly List<Player> activeClones = new();
    
    private bool isMouseHolding;
    private Camera mainCamera;
    private Vector3 targetPosition;

    private void Start()
    {
        mainCamera = Camera.main;
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
                
        targetPosition = new Vector3(worldPosition.x, worldPosition.y, clampedZ);
    }
    
    private void Update()
    {
        if (isMouseHolding)
        {
            MoveClones();
        }
    }
    
    public void SpawnClone()
    {
        var position = Vector3.up;
        
        if (activeClones.Count > 1)
        {
            var randomClonePosition = activeClones[Random.Range(0, activeClones.Count)].transform.position;
            position = new Vector3(
                randomClonePosition.x + Random.Range(-cloneSpacing, cloneSpacing),
                randomClonePosition.y,
                randomClonePosition.z + Random.Range(-cloneSpacing, cloneSpacing)
            );
        }
        
        var clone = Instantiate(clonePrefab, position, Quaternion.identity).GetComponent<Player>();
        activeClones.Add(clone);
        
        clone.OnHealthChanged += UpdateTotalHealth;
        UpdateTotalHealth();
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
        hpText.text = $"Total HP: {GetTotalHealth()}%";
    }
    
    private float GetTotalHealth() => activeClones.Sum(clone => clone.currentHealth);
}