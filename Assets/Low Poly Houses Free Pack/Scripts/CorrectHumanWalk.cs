using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CorrectHumanWalk : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 1.4f;
    public float gravity = -9.8f;

    [Header("Look")]
    public float mouseSensitivity = 120f;
    public float touchSensitivity = 0.15f;
    public float maxPitch = 60f;

    CharacterController controller;
    Transform cam;

    float pitch = 0f;
    float verticalVelocity = 0f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        Camera c = GetComponentInChildren<Camera>();
        if (c == null)
        {
            Debug.LogError("Camera missing under Player");
            enabled = false;
            return;
        }
        cam = c.transform;
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
    }

    // ---------------- CAMERA LOOK ----------------
    void HandleLook()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);

            float yaw = t.deltaPosition.x * touchSensitivity;
            float pitchDelta = t.deltaPosition.y * touchSensitivity;

            pitch -= pitchDelta;
            pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

            cam.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            transform.Rotate(0f, yaw, 0f);
        }
#else
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        pitch -= my;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        cam.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.Rotate(0f, mx, 0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
#endif
    }

    // ---------------- MOVEMENT (STABLE) ----------------
    void HandleMovement()
    {
#if UNITY_ANDROID || UNITY_IOS
        bool walking = Input.touchCount > 0;
#else
        bool walking = Input.GetKey(KeyCode.W);
#endif

        // ✅ FLATTEN FORWARD VECTOR (THIS IS THE FIX)
        Vector3 flatForward = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;

        Vector3 horizontalMove = walking ? flatForward * walkSpeed : Vector3.zero;

        // Gravity handling
        if (controller.isGrounded)
            verticalVelocity = 0f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        Vector3 finalMove = horizontalMove + Vector3.up * verticalVelocity;

        controller.Move(finalMove * Time.deltaTime);
    }
}
