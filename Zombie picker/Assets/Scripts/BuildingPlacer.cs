using UnityEngine;
using System.Collections.Generic;

public class BuildingPlacer : MonoBehaviour
{
    public enum RotationMode
    {
        FaceLineDirection,
        FixedRotation,
        CustomRotationPerPrefab // на будущее, если у каждого будет уникальный поворот
    }

    [Header("Точки начала и конца линии")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Список префабов зданий")]
    public List<GameObject> buildingPrefabs;

    [Header("Настройки")]
    public bool generateOnStart = true;
    public float spacing = 0.5f;
    public RotationMode rotationMode = RotationMode.FaceLineDirection;
    public Vector3 fixedRotationEuler = Vector3.zero; // если выбран FixedRotation

    private List<GameObject> spawnedBuildings = new List<GameObject>();

    void Start()
    {
        if (generateOnStart)
            GenerateBuildings();
    }

    public void GenerateBuildings()
    {
        foreach (var building in spawnedBuildings)
        {
            Destroy(building);
        }
        spawnedBuildings.Clear();

        Vector3 direction = (endPoint.position - startPoint.position).normalized;
        float totalDistance = Vector3.Distance(startPoint.position, endPoint.position);
        float currentDistance = 0f;
        Vector3 currentPos = startPoint.position;

        while (currentDistance < totalDistance)
        {
            GameObject prefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Count)];

            // Получаем размер
            GameObject temp = Instantiate(prefab);
            Renderer rend = temp.GetComponentInChildren<Renderer>();
            float size = rend != null ? rend.bounds.size.x : 1f;
            Destroy(temp);

            if (currentDistance + size > totalDistance)
                break;

            Vector3 spawnPos = currentPos + direction * (size / 2f);
            Quaternion rotation = GetRotation(direction);
            GameObject newBuilding = Instantiate(prefab, spawnPos, rotation, this.transform);
            LoopAlongLine loop = newBuilding.AddComponent<LoopAlongLine>();
            loop.startPoint = startPoint;
            loop.endPoint = endPoint;
            spawnedBuildings.Add(newBuilding);

            currentDistance += size + spacing;
            currentPos = startPoint.position + direction * currentDistance;
        }
    }

    private Quaternion GetRotation(Vector3 direction)
    {
        switch (rotationMode)
        {
            case RotationMode.FaceLineDirection:
                return Quaternion.LookRotation(direction);
            case RotationMode.FixedRotation:
                return Quaternion.Euler(fixedRotationEuler);
            default:
                return Quaternion.identity;
        }
    }
}