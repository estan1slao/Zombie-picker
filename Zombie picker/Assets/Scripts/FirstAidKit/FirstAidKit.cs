using UnityEngine;

public class FirstAidKit : MonoBehaviour
{
    private void Update()
    {
        if (transform.position.x >= 20) 
            Destroy(gameObject);
    }
}