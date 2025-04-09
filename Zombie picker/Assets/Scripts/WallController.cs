using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallController : MonoBehaviour
{
    public int hits;
    public GameObject gStenka;
    public GameObject rStenka;
    public WallController otherWallController;
    public TextMeshProUGUI hitText;
    
    private CloneController cloneController;

    [NonSerialized] public bool IsActive = true;
    
    private void Awake()
    {
        cloneController = GameObject.FindGameObjectWithTag("CloneController").GetComponent<CloneController>();
        UpdateWall();
        hitText.text = hits.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && otherWallController == null &&
            cloneController.GetNearestToMouseClone().gameObject == other.gameObject)
        {
            ProcessWall();
            Destroy(gameObject);
        }
        
        else if (other.CompareTag("Player") && IsActive)
        {
            ProcessWall();
            
            IsActive = false;
            otherWallController.IsActive = false;
            
            Destroy(gameObject);
        }
    }

    private void ProcessWall()
    {
        switch (hits)
        {
            case > 0:
                cloneController.SpawnClones(hits);
                break;
            case < 0:
                cloneController.DestroyClones(Mathf.Abs(hits));
                break;
        }
    }
    
    public void TakeDamage()
    {
        hits++;
        UpdateWall();
        hitText.text = hits.ToString();
    }

    private void UpdateWall()
    {
        switch (hits)
        {
            case >= 0:
                rStenka.SetActive(false);
                gStenka.SetActive(true);
                break;
            case < 0:
                rStenka.SetActive(true);
                gStenka.SetActive(false);
                break;
        }
    }
}