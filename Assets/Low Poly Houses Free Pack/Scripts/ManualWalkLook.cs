using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ManualWalkLook : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 1.4f;   // constant human speed

    [Header("Look")]
    public float mouseSensitivity = 120f;
    public float touchSensitivity = 0.15f;
    public float maxPitch = 60f;

    CharacterController cc;
    Transform cam;

    float pitch = 0f;

    void Awake()
    {
        cc = GetComponent<CharacterController>();

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
        HandleMovement();
        HandleLook();
    }

    // ---------------- MOVEMENT (STABLE) ----------------
    void HandleMovement()
    {
#if UNITY_ANDROID || UNITY_IOS
        // Mobile: walk only when finger is touching (constant speed)
        bool walking = Input.touchCount > 0;
#else
        // Laptop: W key to walk
        bool walking = Input.GetKey(KeyCode.W);
#endif

        if (walking)
        {
            Vector3 move = transform.forward * walkSpeed;
            cc.Move(move * Time.deltaTime);
        }
    }

    // ---------------- LOOK (ROTATION ONLY) ----------------
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
}
