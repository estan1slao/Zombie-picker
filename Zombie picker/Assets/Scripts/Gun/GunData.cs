using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "Gun")]
public class GunData : ScriptableObject
{
    public float damage;
    public float speed;
    public float fireRate;
    public float lifeTime;

    [Header("Visual")]
    public GameObject gunPrefab;
    public Vector3 localPosition;
    public Vector3 localRotation;
}