using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMotor : MonoBehaviour
{
    [System.Serializable]
    public class MovementConfig
    {
        public float moveSpeed = 5f;
        public float jumpForce = 8f;
    }

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Configs")]
    [SerializeField] private MovementConfig jaguarConfig;
    [SerializeField] private MovementConfig condorConfig;
    [SerializeField] private MovementConfig serpienteConfig;
    [SerializeField] private MovementConfig maskConfig;

    private Rigidbody2D rb;

    public bool IsGrounded { get; private set; }
    public bool CanJump { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void TickGround()
    {
        bool wasGrounded = IsGrounded;

        IsGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (IsGrounded && !wasGrounded)
        {
            CanJump = true;
        }
    }

    public void Move(float horizontal, bool lockHorizontal)
    {
        MovementConfig cfg = GetCurrentConfig();

        float vx = lockHorizontal ? 0f : horizontal * cfg.moveSpeed;

        rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);
    }

    public bool TryJump(bool jumpPressed)
    {
        if (!jumpPressed || !CanJump)
        {
            return false;
        }

        MovementConfig cfg = GetCurrentConfig();

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, cfg.jumpForce);
        CanJump = false;
        return true;
    }

    MovementConfig GetCurrentConfig()
    {
        return GameManager.Instance.CurrentForm switch
        {
            GameManager.PlayerForm.Jaguar => jaguarConfig,
            GameManager.PlayerForm.Condor => condorConfig,
            GameManager.PlayerForm.Serpiente => serpienteConfig,
            GameManager.PlayerForm.Mask => maskConfig,
            _ => jaguarConfig,
        };
    }
}