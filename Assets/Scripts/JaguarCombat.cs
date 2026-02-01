using UnityEngine;

[RequireComponent(typeof(Animator))]
public class JaguarCombat : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Jaguar jaguar;

    [Header("Attack Timing")]
    [SerializeField] private float attackTriggerCooldown = 0.3f;
    [SerializeField] private float chargeTimeRequired = 0.8f;

    private Animator animator;

    private float attackTimer;
    private bool canAttack = true;

    private float chargeTimer;
    public bool IsCharging { get; private set; }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Tick(InputManager input)
    {
        UpdateCooldown();

        if (input.ActionHeld)
        {
            StartOrHoldCharge();
        }

        if (input.ActionReleased)
        {
            ReleaseChargeOrAttack();
        }
    }

    void UpdateCooldown()
    {
        if (canAttack)
        {
            return;
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            canAttack = true;
        }
    }

    void StartOrHoldCharge()
    {
        if (!IsCharging)
        {
            IsCharging = true;
            chargeTimer = 0f;
            animator.SetBool("Carga", true);
            return;
        }

        chargeTimer += Time.deltaTime;
    }

    void ReleaseChargeOrAttack()
    {
        if (!IsCharging)
        {
            return;
        }

        animator.SetBool("Carga", false);

        bool chargedAttack = chargeTimer >= chargeTimeRequired;

        if (canAttack)
        {
            animator.SetTrigger("ataq");
            jaguar.TryDestroyObstacle();

            canAttack = false;
            attackTimer = attackTriggerCooldown;
        }

        IsCharging = false;
        chargeTimer = 0f;

        Debug.Log(chargedAttack ? "Ataque CARGADO" : "Ataque normal");
    }
}