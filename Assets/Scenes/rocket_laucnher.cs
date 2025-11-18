using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class rocket_laucnher : MonoBehaviour
{
    public GameObject RocketPrefab;
    public float cooldown = 1f;
    InputAction fire;
    float cooldownTime;

    void Start()
    {
        fire = InputSystem.actions.FindAction("Attack");
    }

    void FixedUpdate()
    {
        cooldownTime += -Time.deltaTime;

        if (fire.WasPerformedThisFrame() && cooldownTime <= 0)
        {
            Instantiate(RocketPrefab, transform.position, transform.rotation);
            cooldownTime = cooldown;
        }
    }
}
