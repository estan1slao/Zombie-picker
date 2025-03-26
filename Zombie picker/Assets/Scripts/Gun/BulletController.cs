using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject bulletPrefab;
    
    public void Shoot(GunData gunData, Vector3 position)
    {
        var bullet = Instantiate(bulletPrefab, position, Quaternion.identity).GetComponent<Bullet>();
        bullet.GunData = gunData;
    }
}