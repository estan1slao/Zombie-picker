using UnityEngine;

[RequireComponent(typeof(Barrel))]
public class FirstAidKitSpawner : MonoBehaviour
{
    public GameObject firstAidKitPrefab;
    private Transform barrelMain;

    private void Start()
    {
        barrelMain = GetComponent<Barrel>().barrelMain.transform;
    }

    public void SpawnFirstAidKit()
    {
        var position = new Vector3(barrelMain.position.x, 0f, barrelMain.position.z);
        Instantiate(firstAidKitPrefab, position, Quaternion.Euler(0, 90, 0));
        Destroy(gameObject);
    }
}
