using UnityEngine;

public class Condor : MonoBehaviour
{
    [Header("Glide Settings")]
    [SerializeField] private float glideDuration = 1.2f;
    [SerializeField] private float horizontalForce = 6f;
    [SerializeField] private float verticalFallSpeed = -1.5f;
    [SerializeField] private float gravityScaleWhileGliding = 0.3f;

    [Header("Animator Params")]
    [SerializeField] private string jumpTrigger = "Salt";
    [SerializeField] private string glideBool = "Vuel";
    [SerializeField] private string groundBool = "Ground";

    private Rigidbody2D rb;
    private Animator animator;

    private float glideTimer;
    private bool isGliding;
    private bool hasGlided;

    private float originalGravity;

    public bool IsGliding => isGliding;

    void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalGravity = rb.gravityScale;
    }

    // =========================
    // JUMP ANIMATION
    // =========================

    public void PlayJumpAnimation()
    {
        if (animator != null && animator.enabled)
        {
            animator.SetTrigger(jumpTrigger);
        }
    }

    // =========================
    // GROUND STATE (OPCIONAL)
    // =========================

    public void SetGrounded(bool grounded)
    {
        if (animator != null && animator.enabled)
        {
            animator.SetBool(groundBool, grounded);
        }

        if (grounded)
        {
            ResetGlide();
        }
    }

    // =========================
    // GLIDE
    // =========================

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

        if (animator != null && animator.enabled)
        {
            animator.SetBool(glideBool, true);
        }
    }

    public void StopGlide()
    {
        if (!isGliding)
        {
            return;
        }

        isGliding = false;
        rb.gravityScale = originalGravity;

        if (animator != null && animator.enabled)
        {
            animator.SetBool(glideBool, false);
        }
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