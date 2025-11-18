using UnityEngine;

public class Rocket_projectile : MonoBehaviour
{
    public float speed = 5;
    public float radius = 2;
    public float power = 500;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= transform.TransformDirection(Vector3.forward * speed) * Time.deltaTime;
        // rb.linearVelocity = transform.forward * (speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other) 
    {
        Debug.Log("TOUCHING");
        kickObjects();
        Destroy(gameObject);
    }

    void kickObjects()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null) rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
        }
    }
}
