using UnityEngine;

public class LoopAlongLine : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float buffer = 2f; // небольшое расстояние, чтобы не сработало слишком рано

    private Vector3 direction;
    private float totalDistance;

    void Start()
    {
        direction = (endPoint.position - startPoint.position).normalized;
        totalDistance = Vector3.Distance(startPoint.position, endPoint.position);
    }

    void Update()
    {
        Vector3 toObject = transform.position - startPoint.position;
        float projection = Vector3.Dot(toObject, direction);

        if (projection < -buffer)
        {
            // Смещаем здание вперёд на длину всей линии
            transform.position += direction * (totalDistance + buffer);
        }
    }
}