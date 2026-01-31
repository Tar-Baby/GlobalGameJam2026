using UnityEngine;

public class Condor : MonoBehaviour
{
    [Header("Glide Settings")]
    [SerializeField] private float glideDuration = 1.2f;
    [SerializeField] private float horizontalForce = 6f;
    [SerializeField] private float verticalFallSpeed = -1.5f;
    [SerializeField] private float gravityScaleWhileGliding = 0.3f;

    private Rigidbody2D rb;
    private float glideTimer;
    private bool isGliding;
    private bool hasGlided;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public bool IsGliding => isGliding;

    public void TryStartGlide(bool isGrounded)
    {
        if (isGrounded || hasGlided || isGliding)
        {
            return;
        }

        StartGlide();
    }

    void StartGlide()
    {
        isGliding = true;
        hasGlided = true;
        glideTimer = glideDuration;

        rb.gravityScale = gravityScaleWhileGliding;
    }

    public void StopGlide()
    {
        if (!isGliding)
        {
            return;
        }

        isGliding = false;
        rb.gravityScale = 1f;
    }

    void FixedUpdate()
    {
        if (!isGliding)
        {
            return;
        }

        glideTimer -= Time.fixedDeltaTime;

        if (glideTimer <= 0f)
        {
            StopGlide();
            return;
        }

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x + horizontalForce * Time.fixedDeltaTime,
            verticalFallSpeed
        );
    }

    public void ResetGlide()
    {
        hasGlided = false;
        StopGlide();
    }
}