using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    public enum AbilityForm
    {
        Triangle,
        Square,
        Circle
    }

    [Header("References")]
    [SerializeField] private InputManager input;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;

    [Header("Abilities - Sprites")]
    [SerializeField] private Sprite triangleSprite;
    [SerializeField] private Sprite squareSprite;
    [SerializeField] private Sprite circleSprite;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    private AbilityForm currentForm;

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
            Debug.Log("Acción con forma: " + currentForm);
        }
    }

    void ChangeAbility(int direction)
    {
        int abilityCount = System.Enum.GetValues(typeof(AbilityForm)).Length;
        int newIndex = ((int)currentForm + direction + abilityCount) % abilityCount;

        SetAbility((AbilityForm)newIndex);
    }

    void SetAbility(AbilityForm newForm)
    {
        currentForm = newForm;

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
                break;
        }

        Debug.Log("Forma actual: " + currentForm);
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