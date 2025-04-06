using System;
using UnityEngine;

public class MineSpawner : MonoBehaviour
{
    public GameObject minePrefab;
    public float mineOffset = 5f;
    
    private void SpawnMines()
    {
        var lastPosition = new Vector3(transform.position.x, 0f, 0f);
        
        for (var i = 0; i < 10; i++)
        {
            Instantiate(minePrefab, lastPosition, Quaternion.Euler(0, 90, 0));
            lastPosition -= new Vector3(mineOffset, 0f, 0f);
        }
        
        Destroy(gameObject);
    }
}