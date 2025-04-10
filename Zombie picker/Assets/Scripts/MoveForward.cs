using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float speed = 5f;
    
    public Vector3 direction = Vector3.forward;
    
    private void Update()
    {
        Move();
        if (transform.position.x >= 20)
            Destroy(gameObject);
    }
    
    private void Move()
    {
        transform.Translate(direction * (speed * Time.deltaTime), Space.World);
    }
}