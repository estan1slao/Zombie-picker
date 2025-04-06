using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float speed = 5f;
    
    private void Update()
    {
        Move();
    }
    
    private void Move()
    {
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
    }
}