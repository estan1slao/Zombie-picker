using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewGun", menuName = "Gun")]
public class GunData : ScriptableObject
{
    public float damage;
    public float speed;
    public float fireRate;
    public float lifeTime;

    [Header("Visual3D")]
    public GameObject gunPrefab;
    public Vector3 localPosition;
    public Vector3 localRotation;

    [Header("Visual2D")] 
    public Image iconImage;

    [Header("Audio")] 
    public AudioClip audioClip;
}