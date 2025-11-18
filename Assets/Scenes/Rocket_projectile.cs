using UnityEngine;

public class Rocket_projectile : MonoBehaviour
{
    public float speed = 20;
    public float radius = 1;
    public float power = 500;

    // Update is called once per frame
    void Update()
    {
        transform.position -= transform.TransformDirection(Vector3.forward * speed) * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other) 
    {
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
