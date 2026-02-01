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
        Triangle, // Cóndor
        Square, // Jaguar
        Circle, // Serpiente
        Mask // Huma
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

    [Header("References")] [SerializeField]
    private InputManager input;

    [SerializeField] private Jaguar jaguar;
    [SerializeField] private Condor condor;
    [SerializeField] private Serpiente serpiente;
    [SerializeField] private MusicManager musicManager;

    [Header("Ground Check")] [SerializeField]
    private Transform groundCheck;

    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Configs")] [SerializeField]
    private MovementConfig triangleConfig;

    [SerializeField] private MovementConfig squareConfig;
    [SerializeField] private MovementConfig circleConfig;
    [SerializeField] private MovementConfig maskConfig;

    [Header("Sprites")] [SerializeField] private Sprite triangleSprite;
    [SerializeField] private Sprite squareSprite;
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private Sprite maskSprite;

    [Header("Animation")] [SerializeField] private Animator animator;

    // =========================
    // PRIVATE STATE
    // =========================

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private AbilityForm currentForm;
    private bool isGrounded;
    private bool canJump;

    [Header("Animation Timing")] [SerializeField]
    private float saltTriggerCooldown = 0.25f;

    private float saltTriggerTimer;
    private bool canTriggerSalt = true;

    [Header("Square Attack Animation")]
    [SerializeField] private float attackTriggerCooldown = 0.3f;
    [SerializeField] private float chargeTimeRequired = 0.8f;

    private float attackTriggerTimer;
    private bool canTriggerAttack = true;

    private float chargeTimer;
    private bool isCharging;

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
        SetAbility(AbilityForm.Square);
    }

    void Update()
    {
        CheckGround();
        HandleJump();
        HandleAbilities();
        HandleAction();
        UpdateAnimation();
        UpdateSaltTriggerTimer();
        UpdateAttackTriggerTimer();
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

        animator.SetBool("Izq", horizontal < -0.01f);
        animator.SetBool("Der", horizontal > 0.01f);

        if (currentForm == AbilityForm.Square && isCharging)
        {
            animator.SetBool("Izq", false);
            animator.SetBool("Der", false);
            return;
        }
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
                condor.ResetGlide();
        }
    }

    // =========================
    // MOVEMENT
    // =========================

    void HandleMovement()
    {
        // Bloquear movimiento lateral si el cuadrado está cargando ataque
        if (currentForm == AbilityForm.Square && isCharging)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );
            return;
        }

        float speed = GetCurrentConfig().moveSpeed;

        rb.linearVelocity = new Vector2(
            input.Horizontal * speed,
            rb.linearVelocity.y
        );
    }

    void HandleJump()
    {
        if (condor != null && condor.IsGliding)
            return;

        if (!input.JumpPressed)
            return;

        if (canJump)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                GetCurrentConfig().jumpForce
            );

            canJump = false;

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
            return;

        saltTriggerTimer -= Time.deltaTime;

        if (saltTriggerTimer <= 0f)
            canTriggerSalt = true;
    }

    // =========================
    // ABILITIES
    // =========================

    void HandleAbilities()
    {
        if (input.AbilityLeftPressed)
            ChangeAbility(-1);

        if (input.AbilityRightPressed)
            ChangeAbility(1);
    }

    void HandleAction()
    {
        if (currentForm != AbilityForm.Square)
        {
            return;
        }

        // Mantener botón → cargar
        if (input.ActionHeld)
        {
            HandleCharge();
            return;
        }

        // Soltar botón
        if (input.ActionReleased)
        {
            ReleaseChargeOrAttack();
        }
    }

    void HandleCharge()
    {
        if (isCharging)
        {
            chargeTimer += Time.deltaTime;
            return;
        }

        isCharging = true;
        chargeTimer = 0f;

        if (animator.enabled)
        {
            animator.SetBool("Carga", true);
        }
    }

    void ReleaseChargeOrAttack()
    {
        if (animator.enabled)
        {
            animator.SetBool("Carga", false);
        }

        bool chargedAttack = chargeTimer >= chargeTimeRequired;

        if (canTriggerAttack)
        {
            animator.SetTrigger("ataq");

            canTriggerAttack = false;
            attackTriggerTimer = attackTriggerCooldown;
        }

        isCharging = false;
        chargeTimer = 0f;

        // Aquí luego puedes diferenciar daño normal vs cargado
        if (chargedAttack)
        {
            Debug.Log("Ataque CARGADO");
        }
        else
        {
            Debug.Log("Ataque normal");
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

        animator.SetBool("Carga", false);
        isCharging = false;
        chargeTimer = 0f;

        currentForm = newForm;

        serpiente.DisablePhase();
        animator.enabled = false;

        switch (currentForm)
        {
            case AbilityForm.Mask: // Huma
                animator.SetBool("Vuel", false);
                animator.enabled = true;
                spriteRenderer.sprite = null;
                break;

            case AbilityForm.Triangle: // Cóndor
                animator.enabled = true;
                spriteRenderer.sprite = null;
                break;

            case AbilityForm.Square: // Jaguar
                animator.SetBool("Vuel", false);
                animator.enabled = true;
                spriteRenderer.sprite = null;
                break;

            case AbilityForm.Circle: // Serpiente
                animator.SetBool("Vuel", false);
                spriteRenderer.sprite = circleSprite;
                serpiente.EnablePhase();
                break;
        }

        ChangeMusicByForm(currentForm);

        Debug.Log("Forma actual: " + currentForm);
    }

    void ChangeMusicByForm(AbilityForm form)
    {
        if (musicManager == null)
            return;

        musicManager.ChangeMusic(form);
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

    void UpdateAttackTriggerTimer()
    {
        if (canTriggerAttack)
        {
            return;
        }

        attackTriggerTimer -= Time.deltaTime;

        if (attackTriggerTimer <= 0f)
        {
            canTriggerAttack = true;
        }
    }

    // =========================
    // DEBUG
    // =========================

    void OnDrawGizmosSelected()
    {
    }
}