using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    // =========================
    // ENUMS & DATA
    // =========================

    public enum AbilityForm
    {
        Triangle,
        Square,
        Circle,
        Mask
    }

    [System.Serializable]
    public class MovementConfig
    {
        public float moveSpeed;
        public float jumpForce;
    }

    // =========================
    // INSPECTOR REFERENCES
    // =========================

    [Header("References")]
    [SerializeField] private InputManager input;
    [SerializeField] private Jaguar jaguar;
    [SerializeField] private Condor condor;
    [SerializeField] private Serpiente serpiente;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Configs")]
    [SerializeField] private MovementConfig triangleConfig;
    [SerializeField] private MovementConfig squareConfig;
    [SerializeField] private MovementConfig circleConfig;
    [SerializeField] private MovementConfig maskConfig;

    [Header("Sprites")]
    [SerializeField] private Sprite triangleSprite;
    [SerializeField] private Sprite squareSprite;
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private Sprite maskSprite;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    // =========================
    // PRIVATE STATE
    // =========================

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private AbilityForm currentForm;
    private bool isGrounded;
    private bool canJump;

    [Header("Animation Timing")]
    [SerializeField] private float saltTriggerCooldown = 0.25f;

    private float saltTriggerTimer;
    private bool canTriggerSalt = true;

    // =========================
    // UNITY LIFECYCLE
    // =========================

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        SetAbility(AbilityForm.Triangle);
    }

    void Update()
    {
        CheckGround();
        HandleJump();
        HandleAbilities();
        HandleAction();
        UpdateAnimation();
        UpdateSaltTriggerTimer();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    // =========================
    // ANIMATIONS
    // =========================

    void UpdateAnimation()
    {
        float horizontal = input.Horizontal;

        bool goingLeft = horizontal < -0.01f;
        bool goingRight = horizontal > 0.01f;

        animator.SetBool("Izq", goingLeft);
        animator.SetBool("Der", goingRight);
    }
    
    // =========================
    // GROUND CHECK
    // =========================

    void CheckGround()
    {
        bool wasGrounded = isGrounded;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (isGrounded && !wasGrounded)
        {
            canJump = true;

            if (currentForm == AbilityForm.Triangle)
            {
                condor.ResetGlide();
            }
        }
    }

    // =========================
    // MOVEMENT
    // =========================

    void HandleMovement()
    {
        float speed = GetCurrentConfig().moveSpeed;

        rb.linearVelocity = new Vector2(
            input.Horizontal * speed,
            rb.linearVelocity.y
        );
    }

    void HandleJump()
    {
        // Si estamos planeando, no saltamos
        if (condor != null && condor.IsGliding)
        {
            return;
        }

        if (!input.JumpPressed)
        {
            return;
        }

        if (canJump)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                GetCurrentConfig().jumpForce
            );

            canJump = false;

            // 🔥 Trigger Salt con bounce time
            if (
                currentForm == AbilityForm.Triangle &&
                animator.enabled &&
                canTriggerSalt
            )
            {
                animator.SetTrigger("Salt");

                canTriggerSalt = false;
                saltTriggerTimer = saltTriggerCooldown;
            }
        }
        else if (currentForm == AbilityForm.Triangle)
        {
            condor.TryStartGlide(isGrounded);
        }
    }

    void UpdateSaltTriggerTimer()
    {
        if (canTriggerSalt)
        {
            return;
        }

        saltTriggerTimer -= Time.deltaTime;

        if (saltTriggerTimer <= 0f)
        {
            canTriggerSalt = true;
        }
    }

    // =========================
    // ABILITIES
    // =========================

    void HandleAbilities()
    {
        if (input.AbilityLeftPressed)
        {
            ChangeAbility(-1);
        }

        if (input.AbilityRightPressed)
        {
            ChangeAbility(1);
        }
    }

    void HandleAction()
    {
        if (!input.ActionPressed)
        {
            return;
        }

        if (currentForm == AbilityForm.Square)
        {
            jaguar.TryDestroyObstacle();
        }
        else
        {
            Debug.Log("Acción no disponible para " + currentForm);
        }
    }

    // =========================
    // FORM MANAGEMENT
    // =========================

    void ChangeAbility(int direction)
    {
        int count = System.Enum.GetValues(typeof(AbilityForm)).Length;
        int newIndex = ((int)currentForm + direction + count) % count;

        SetAbility((AbilityForm)newIndex);
    }

    void SetAbility(AbilityForm newForm)
    {
        currentForm = newForm;

        // Reset habilidades
        serpiente.DisablePhase();

        // Por defecto: sin animaciones
        animator.enabled = false;

        switch (currentForm)
        {
            case AbilityForm.Mask:
                animator.SetBool("Vuel", false);
                animator.enabled = true;
                spriteRenderer.sprite = null;
                break;

            case AbilityForm.Triangle:
                animator.enabled = true;
                spriteRenderer.sprite = null;
                break;

            case AbilityForm.Square:
                animator.SetBool("Vuel", false);
                spriteRenderer.sprite = squareSprite;
                break;

            case AbilityForm.Circle:
                animator.SetBool("Vuel", false);
                spriteRenderer.sprite = circleSprite;
                serpiente.EnablePhase();
                break;
        }

        Debug.Log("Forma actual: " + currentForm);
    }

    MovementConfig GetCurrentConfig()
    {
        switch (currentForm)
        {
            case AbilityForm.Mask:
                return maskConfig;

            case AbilityForm.Triangle:
                return triangleConfig;

            case AbilityForm.Square:
                return squareConfig;

            case AbilityForm.Circle:
                return circleConfig;

            default:
                return triangleConfig;
        }
    }

    // =========================
    // DEBUG
    // =========================

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}