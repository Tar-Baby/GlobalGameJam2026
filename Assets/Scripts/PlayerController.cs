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
        Circle
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

    [Header("Sprites")]
    [SerializeField] private Sprite triangleSprite;
    [SerializeField] private Sprite squareSprite;
    [SerializeField] private Sprite circleSprite;

    // =========================
    // PRIVATE STATE
    // =========================

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private AbilityForm currentForm;
    private bool isGrounded;
    private bool canJump;

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
    }

    void FixedUpdate()
    {
        HandleMovement();
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

        if (input.JumpPressed)
        {
            if (canJump)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    GetCurrentConfig().jumpForce
                );

                canJump = false;
            }
            else if (currentForm == AbilityForm.Triangle)
            {
                condor.TryStartGlide(isGrounded);
            }
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

        // Desactivar todas las habilidades de fase
        serpiente.DisablePhase();

        switch (currentForm)
        {
            case AbilityForm.Triangle:
                spriteRenderer.sprite = triangleSprite;
                break;

            case AbilityForm.Square:
                spriteRenderer.sprite = squareSprite;
                break;

            case AbilityForm.Circle:
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