using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    public float Xsensitivity = 1f;
    public float Ysensitivity = 1f;

    public Transform playerTransform; // reference to your player object (the one that moves)

    float xRot = 0f;
    float yRot = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Mouse.current.delta.x.ReadValue() * Xsensitivity; //* Time.deltaTime;
        float mouseY = Mouse.current.delta.y.ReadValue() * Ysensitivity; //* Time.deltaTime;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -89f, 89f);
        yRot += mouseX;

        // if (Mathf.Abs(mouseX) > 0.001f || Mathf.Abs(mouseY) > 0.001f)
           // Debug.Log($"Mouse input: X={mouseX:F3}, Y={mouseY:F3}");

        // vertical look
        transform.localRotation = Quaternion.Euler(xRot, yRot, 0f);

        // rotate player transform
        playerTransform.Rotate(Vector3.up * mouseX);
    }
}
