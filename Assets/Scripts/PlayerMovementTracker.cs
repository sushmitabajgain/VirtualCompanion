using UnityEngine;

public class PlayerMovementTracker : MonoBehaviour
{
    public static PlayerMovementTracker Instance;

    [Header("Movement Detection")]
    public float stillThreshold = 0.01f;
    public float stillTimeRequired = 2f;
    public float movementSpeakCooldown = 1.5f;

    Vector3 lastPosition;
    float stillTimer;
    float lastMoveTime;

    public bool IsPlayerStill { get; private set; }
    public bool IsPlayerMoving => !IsPlayerStill;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, lastPosition);

        if (distance < stillThreshold)
        {
            stillTimer += Time.deltaTime;

            if (stillTimer >= stillTimeRequired)
                IsPlayerStill = true;
        }
        else
        {
            stillTimer = 0f;
            IsPlayerStill = false;
            lastMoveTime = Time.time;
        }

        lastPosition = transform.position;
    }

    public bool CanSpeakWhileMoving()
    {
        return Time.time - lastMoveTime > movementSpeakCooldown;
    }
}
