using UnityEngine;
using System.Collections;

public class CameraGestureController : MonoBehaviour
{
    // ====== Inspector References ======
    [Header("References")]
    public Camera cam;                 // Main camera reference
    public Transform pitchPivot;       // Pivot for vertical rotation (pitch)

    // ====== Movement Settings ======
    [Header("Move (hold to go forward)")]
    public float moveSpeed = 5f;       // Forward movement speed
    public LayerMask worldMask = ~0;   // Defines ground collision layers
    public float groundProbeUp = 2f, groundProbeDown = 5f; // Distances used for ground snapping

    // ====== Look (Camera Rotation) ======
    [Header("Look")]
    public float yawSpeed = 0.12f, pitchSpeed = 0.12f; // Sensitivity of rotation
    public float minPitch = -70f, maxPitch = 70f;      // Clamp vertical rotation

    // ====== Zoom (Field of View) ======
    [Header("Zoom (FOV)")]
    public float minFov = 45f, maxFov = 80f;           // Min and max zoom levels
    public float pinchToFov = 0.08f;                   // Pinch sensitivity

    // ====== Gesture Tuning ======
    [Header("Gesture tuning")]
    public float dragDeadzonePx = 6f;                  // Deadzone for detecting drag
    public float holdToMoveDelay = 0.1f;               // Delay before hold triggers forward movement

    // ====== Internal State ======
    CharacterController cc;         // Handles player movement/collision
    float yaw, pitch;                // Current rotation values
    float lastPinchDist = -1f;       // Used to track pinch gesture distance

    // One-finger interaction modes
    enum OneFingerMode { None, PendingMove, Moving, Looking }
    OneFingerMode mode = OneFingerMode.None;

    // Finger touch tracking
    Vector2 startPos;      
    float   startTime;      

    // Whether ground snapping is primed after physics update
    bool primed;           


    // ====== Initialization ======
    void Awake()
    {
        // Ensure CharacterController and references are set
        cc = GetComponent<CharacterController>();
        if (!cc) cc = gameObject.AddComponent<CharacterController>();
        if (!cam) cam = Camera.main;
        if (!pitchPivot) pitchPivot = cam.transform;

        // Initialize yaw/pitch based on current camera orientation
        Vector3 eCam = pitchPivot.rotation.eulerAngles;
        yaw   = Normalize(eCam.y);
        pitch = Mathf.Clamp(Normalize(eCam.x), minPitch, maxPitch);

        // Snap to ground and prepare for movement
        InitialSnapExact();
        StartCoroutine(PrimeAfterPhysics());
    }

    // Delay priming ground snapping until physics step has passed
    IEnumerator PrimeAfterPhysics() { yield return new WaitForFixedUpdate(); primed = true; }


    // ====== Per-frame Update ======
    void Update()
    {
        HandleTouchesWithIntent();  // Handle touch/mouse gestures
        MoveIfNeeded();             // Move if in movement mode
        if (primed) SnapToGroundExact(); // Keep player aligned with ground
    }


    // ====== Gesture Handling ======
    void HandleTouchesWithIntent()
    {
        int n = Input.touchCount;

        // --- Two-finger pinch: zoom (change FOV) ---
        if (n >= 2)
        {
            mode = OneFingerMode.None;

            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            // Reset pinch distance if new touch began
            if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began || lastPinchDist < 0f)
            {
                lastPinchDist = Vector2.Distance(t0.position, t1.position);
                return; 
            }

            // Calculate pinch delta and adjust camera FOV
            float dist  = Vector2.Distance(t0.position, t1.position);
            float delta = dist - lastPinchDist;
            if (Mathf.Abs(delta) > 0.0f)
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - delta * pinchToFov, minFov, maxFov);

            lastPinchDist = dist;
        }
        // --- One-finger gestures: move/look ---
        else if (n == 1)
        {
            Touch t0 = Input.GetTouch(0);

            if (t0.phase == TouchPhase.Began)
            {
                // Start in "PendingMove" state
                mode = OneFingerMode.PendingMove;   
                startPos  = t0.position;
                startTime = Time.time;
                lastPinchDist = -1f;
            }
            else if (t0.phase == TouchPhase.Moved || t0.phase == TouchPhase.Stationary)
            {
                // Decide if gesture is look or move
                if (mode == OneFingerMode.PendingMove)
                {
                    if ((t0.position - startPos).magnitude >= dragDeadzonePx)
                        mode = OneFingerMode.Looking; // Drag → look
                    else if (Time.time - startTime >= holdToMoveDelay)
                        mode = OneFingerMode.Moving;  // Hold → move forward
                }

                // Apply look or adjust while moving
                if (mode == OneFingerMode.Looking || (mode == OneFingerMode.Moving && t0.phase == TouchPhase.Moved))
                {
                    Vector2 d = t0.deltaPosition;
                    yaw   += d.x * yawSpeed;
                    pitch -= d.y * pitchSpeed;
                    pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);
                }
            }
            else
            {
                // Finger lifted → reset mode
                mode = OneFingerMode.None;
            }
        }
        else
        {
            // No touches
            mode = OneFingerMode.None;
            lastPinchDist = -1f;
        }

        // Apply yaw/pitch rotations
        transform.rotation       = Quaternion.Euler(0f, yaw, 0f);
        pitchPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

#if UNITY_EDITOR
        // --- Mouse controls for editor testing ---
        if (Input.GetMouseButton(1)) // Right mouse drag → look
        {
            yaw   += Input.GetAxis("Mouse X") * 5f;
            pitch -= Input.GetAxis("Mouse Y") * 5f;
            pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        if (Input.GetMouseButtonDown(0)) { mode = OneFingerMode.PendingMove; startTime = Time.time; startPos = Input.mousePosition; }
        if (Input.GetMouseButton(0))
        {
            if (mode == OneFingerMode.PendingMove && (Time.time - startTime) >= holdToMoveDelay)
                mode = OneFingerMode.Moving;
        }
        if (Input.GetMouseButtonUp(0)) mode = OneFingerMode.None;

        // Mouse wheel scroll → zoom
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0f) cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - scroll * 2f, minFov, maxFov);
#endif
    }


    // ====== Forward Movement ======
    void MoveIfNeeded()
    {
        if (mode != OneFingerMode.Moving) return;
        Vector3 fwd = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        cc.Move(fwd * moveSpeed * Time.deltaTime);
    }


    // ====== Ground Snapping ======
    void InitialSnapExact()
    {
        // Snap player to ground at start
        bool wasEnabled = cc.enabled; cc.enabled = false;
        Vector3 start = transform.position + Vector3.up * groundProbeUp;
        if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, groundProbeUp + groundProbeDown, worldMask,
                            QueryTriggerInteraction.Ignore))
        {
            float bottomToOrigin = cc.center.y - cc.height * 0.5f; 
            float targetY = hit.point.y - bottomToOrigin + cc.skinWidth;
            transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
        }
        cc.enabled = wasEnabled;
    }

    void SnapToGroundExact()
    {
        // Continuous snapping during runtime
        Vector3 start = transform.position + Vector3.up * groundProbeUp;
        if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, groundProbeUp + groundProbeDown, worldMask,
                            QueryTriggerInteraction.Ignore))
        {
            float bottomToOrigin = cc.center.y - cc.height * 0.5f;
            float targetY = hit.point.y - bottomToOrigin + cc.skinWidth;
            float dy = targetY - transform.position.y;
            if (Mathf.Abs(dy) > 0.0001f)
                cc.Move(new Vector3(0f, dy, 0f));
        }
    }

    // ====== Utility ======
    static float Normalize(float a) => (a > 180f) ? a - 360f : a;
}
