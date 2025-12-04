using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Character_controller : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    public float WalkSpeed = 5;
    public AnimationCurve walkAcceleration;
    public AnimationCurve airAcceleration;
    public float floorFriction = 7f;

    public float maxFloorDeg = 40; // don't update during the game
    private float maxFloorCos = 0f;

    public int airJumps = 1;
    public float coyoteTime = 0.2f;

    public Transform PlayerTransform;
    public Rigidbody rb;

    public TMP_Text  floorLabel;

    private bool isOnFloor = true;
    private Vector3 floorNormal = new Vector3(0f, 0f, 0f);
    private int jumpsLeft;
    private float coyoteTimeLeft;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        maxFloorCos = Mathf.Cos(maxFloorDeg * Mathf.Deg2Rad);
        Debug.Assert(PlayerTransform);
    }

    private void OnCollisionStay(Collision collision)
    {
        Vector3 averageNormal = Vector3.zero;
        int floorContactCount = 0;

        // Iterate through all contact points for current collision
        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 normal = contact.normal;
            float dotProduct = Vector3.Dot(Vector3.up, normal); // gives cos(angle)

            if (dotProduct > maxFloorCos)
            {
                averageNormal += normal;
                floorContactCount++;
            }
        }

        if (floorContactCount > 0)
        {
            floorNormal = averageNormal.normalized;
            isOnFloor = true;
        }
        else
        {
            isOnFloor = false;
        }
    }
    void FixedUpdate()
    {
        movement();
        jump();
        isOnFloor = false;
    }
    void movement()
    {
        rb.useGravity = true;
        // get player input
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        // Debug.Log(move.ToString());
        Vector3 direction = PlayerTransform.transform.TransformDirection(move);

        if (isOnFloor)
        {
            apply_friction(floorFriction);
            rb.useGravity = false;
        }
        if (direction == Vector3.zero) return;

        // quake 1 style movement + my extra
        float wishSpeed = Mathf.Min(moveInput.magnitude, 1f) * WalkSpeed;
        float currentSpeed = Vector3.Dot(rb.linearVelocity, direction);

        // how uch more speed to add
        float addSpeed = wishSpeed - currentSpeed; // how much to add
        // Debug.Log(addSpeed);
        if (addSpeed <= 0f) return; // already at speed

        float speedFraction = Mathf.Clamp(rb.linearVelocity.magnitude / WalkSpeed, 0f, 1f);

        float accelSpeed;
        if (isOnFloor)
        {
            accelSpeed = (walkAcceleration.Evaluate(speedFraction)*10 ) * Time.deltaTime * wishSpeed;
            // Debug.Log(walkAcceleration.Evaluate(speedFraction) * Time.deltaTime * wishSpeed);
        }
        else
        {
            accelSpeed = (airAcceleration.Evaluate(speedFraction)*10 ) * Time.deltaTime * wishSpeed;
        }

        if (accelSpeed > addSpeed)
        {
            accelSpeed = addSpeed;
        }
        rb.linearVelocity += direction * accelSpeed;
    }
    
    void jump()
    {
        coyoteTimeLeft += Time.deltaTime;

        if (floorLabel) floorLabel.text = jumpsLeft.ToString();
        if (isOnFloor)
        {
            jumpsLeft = airJumps;
            coyoteTimeLeft = coyoteTime;
        }

        if (!inputActions.Player.Jump.WasPerformedThisFrame())
        {
            return;
        }

        Vector3 jumpVelocity = new Vector3(rb.linearVelocity.x, 5f, rb.linearVelocity.z); 
        if (isOnFloor)
        {
            rb.linearVelocity = jumpVelocity;
            coyoteTimeLeft = -1f;
            isOnFloor = false;
        } 
        else if (coyoteTimeLeft > 0f) 
        {
            rb.linearVelocity = jumpVelocity;
            coyoteTimeLeft = -1f;
            Debug.Log(coyoteTimeLeft.ToString());
        } 
        else if (jumpsLeft > 0)
        {
            rb.linearVelocity = jumpVelocity;
            jumpsLeft += -1;
        }
    }

    void apply_friction(float friction)
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float speed = horizontalVelocity.magnitude;
        if (speed < .01f)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        } else
        {
            float drop = speed * friction * Time.deltaTime;
            rb.linearVelocity *= Mathf.Max(speed - drop, 0f) / speed;
        }
    }
}
