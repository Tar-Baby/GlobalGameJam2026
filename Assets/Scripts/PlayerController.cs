using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputManager input;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleJump();
        HandleAbilities();
        HandleAction();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        rb.linearVelocity = new Vector2(
            input.Horizontal * moveSpeed,
            rb.linearVelocity.y
        );
    }

    void HandleJump()
    {
        if (input.JumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );
        }
    }

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
        if (input.ActionPressed)
        {
            PerformAction();
        }
    }

    void ChangeAbility(int direction)
    {
        Debug.Log("Cambio de habilidad: " + direction);
    }

    void PerformAction()
    {
        Debug.Log("Acción ejecutada");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}