using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 0.15f;
    [SerializeField] private BoxCollider2D bounds;

    private Camera cam;
    private float halfHeight;
    private float halfWidth;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        CalculateCameraSize();
    }

    void LateUpdate()
    {
        if (target == null || bounds == null)
        {
            return;
        }

        FollowTarget();
    }

    void CalculateCameraSize()
    {
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;
    }

    void FollowTarget()
    {
        Vector3 desiredPosition = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        float minX = bounds.bounds.min.x + halfWidth;
        float maxX = bounds.bounds.max.x - halfWidth;
        float minY = bounds.bounds.min.y + halfHeight;
        float maxY = bounds.bounds.max.y - halfHeight;

        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed
        );
    }
}