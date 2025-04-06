using UnityEngine;

public class FirstAidKit : MonoBehaviour
{
    public float speed = 5f;

    private void Update()
    {
        MoveForward();
        
        if (transform.position.x >= 20) 
            Destroy(gameObject);
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
    }
}