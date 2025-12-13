using UnityEngine;
#if UNITY_ANDROID || UNITY_IOS
using Google.XR.Cardboard;
#endif

public class CardboardHeadMoveController : MonoBehaviour
{
    // Reference to the main camera (player's head)
    public Camera cam;               

    // Reference to Unity's CharacterController (handles movement & collisions)
    public CharacterController cc;    

    // Movement speed when moving forward
    public float moveSpeed = 2.2f;

    // Defines which layers in the scene are considered "world" for ground detection
    public LayerMask worldMask = ~0; 

    // Vertical offset for player's eye height
    public float eyeHeight = 0f;      

    // Distances above and below the character used for ground probing
    public float probeUp = 2f, probeDown = 5f;


    // Called when script is reset in Inspector – automatically assigns main camera & character controller
    void Reset(){ cam = Camera.main; cc = GetComponent<CharacterController>(); }

    // Called at game start – snaps player to ground
    void Start(){ SnapToGround(); }   


    // Called once per frame – handles input and movement
    void Update()
    {
        // If camera or controller is missing, stop
        if (!cam || !cc) return;

        // Detect if "move forward" input is pressed:
        // - On mobile VR (Cardboard), trigger press or touch
        // - On PC, left mouse button
        bool pressed =
#if UNITY_ANDROID || UNITY_IOS
            Google.XR.Cardboard.Api.IsTriggerPressed || Input.touchCount > 0;
#else
            Input.GetMouseButton(0);
#endif

        // If pressed, move player in the forward direction of camera (ignoring vertical tilt)
        if (pressed)
        {
            Vector3 flatFwd = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
            cc.Move(flatFwd * moveSpeed * Time.deltaTime);
        }

        // Ensure player stays aligned with ground surface
        SnapToGround();
    }


    // Keeps character snapped to ground, even on uneven surfaces
    void SnapToGround()
    {
        // Start point above character for raycasting downwards
        Vector3 start = transform.position + Vector3.up * probeUp;

        // Total distance raycast will cover (above + below)
        float castDist = probeUp + probeDown;

        // Perform downward raycast to find ground
        if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, castDist, worldMask, QueryTriggerInteraction.Ignore))
        {
            // Calculate bottom offset of character collider
            float bottomToOrigin = cc.center.y - cc.height * 0.5f;

            // Compute target y-position aligned with ground, adjusted by eye height and skin width
            float targetY = hit.point.y - bottomToOrigin + cc.skinWidth + eyeHeight;

            // Find vertical difference from current position
            float dy = targetY - transform.position.y;

            // Apply movement if difference is significant
            if (Mathf.Abs(dy) > 0.0001f) cc.Move(new Vector3(0f, dy, 0f));
        }
    }
}
