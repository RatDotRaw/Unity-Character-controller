using UnityEngine;

public class Rocket_projectile : MonoBehaviour
{
    public float speed = 5;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= transform.TransformDirection(Vector3.forward * speed) * Time.deltaTime;
    }
}
