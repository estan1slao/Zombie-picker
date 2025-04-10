using UnityEngine;

public class DestroyOutBounds : MonoBehaviour
{
    private void Update()
    {
        if (transform.position.x >= 20)
            Destroy(gameObject);
    }
}