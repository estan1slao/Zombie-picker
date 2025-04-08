using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Barrel : MonoBehaviour
{
    [SerializeField] private Image mineIcon;
    [NonSerialized] private int hits = 10;
    
    public int maxHits = 10;
    public float damage = 100f;
    
    public Image barrelImage;
    
    public TextMeshProUGUI hitText;

    public GameObject barrelMain;

    public UnityEvent breakEvent;
    
    private void Start()
    {
        hits = maxHits;
        hitText.text = maxHits.ToString();

        if (TryGetComponent<GunSpawner>(out var gun))
        {
            barrelImage.sprite = gun.gunData.iconImage.sprite;
        }

        if (TryGetComponent<MineSpawner>(out var mine))
        {
            barrelImage.sprite = mineIcon.sprite;
        }
    }

    private void Update()
    {
        if (transform.position.x >= 20)
            Destroy(barrelMain);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Clone>().TakeDamage(damage);
        }
    }

    public void TakeDamage()
    {
        hits = Mathf.Clamp(hits-1, 0, maxHits);
        
        hitText.text = hits.ToString();
        
        if (hits <= 0) 
            Break();
    }

    private void Break()
    {
        breakEvent.Invoke();
        Destroy(barrelMain);
    }
}