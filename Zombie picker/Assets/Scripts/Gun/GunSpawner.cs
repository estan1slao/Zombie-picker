using UnityEngine;

[RequireComponent(typeof(Barrel))]
public class GunSpawner : MonoBehaviour
{
    public GunData gunData;
    
    private Transform barrelMain;

    private void Start()
    {
        barrelMain = GetComponent<Barrel>().barrelMain.transform;
    }
    
    public void SpawnGun()
    {
        var gunObject = Instantiate(gunData.gunPrefab, barrelMain.position, Quaternion.Euler(0, 0, 0));
        
        gunObject.AddComponent<Gun>().GunData = gunData;
        gunObject.AddComponent<MoveForward>().direction = Vector3.right;
        var meshCollider = gunObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
        
        Destroy(gameObject);
    }
}