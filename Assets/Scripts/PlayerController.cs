using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputManager input;
    [SerializeField] private PlayerMotor motor;
    [SerializeField] private PlayerFormSwitcher forms;

    void Update()
    {
        motor.TickGround();
        // ✅ Condor: estado de suelo (Ground bool)
        if (GameManager.Instance.CurrentForm == GameManager.PlayerForm.Condor)
        {
            forms.ActiveCondor?.SetGrounded(motor.IsGrounded);
        }

        if (input.AbilityLeftPressed)
        {
            GameManager.Instance.CyclePlayerForm(-1);
        }

        if (input.AbilityRightPressed)
        {
            GameManager.Instance.CyclePlayerForm(1);
        }

        // Jump (primario)
        bool jumped = motor.TryJump(input.JumpPressed);
        // ✅ Condor: animación de salto
        if (
            jumped &&
            GameManager.Instance.CurrentForm == GameManager.PlayerForm.Condor
        )
        {
            forms.ActiveCondor?.PlayJumpAnimation();
        }

        // Condor: segundo toque en el aire para planear
        if (
            !jumped &&
            GameManager.Instance.CurrentForm == GameManager.PlayerForm.Condor &&
            input.JumpPressed
        )
        {
            forms.ActiveCondor?.TryStartGlide(motor.IsGrounded);
        }

        // Jaguar: combate/carga/ataque (su propio script)
        if (GameManager.Instance.CurrentForm == GameManager.PlayerForm.Jaguar)
        {
            forms.ActiveJaguarCombat?.Tick(input);
        }

        UpdateDirectionBools();
    }

    void FixedUpdate()
    {
        bool lockHorizontal = false;

        if (
            GameManager.Instance.CurrentForm == GameManager.PlayerForm.Jaguar &&
            forms.ActiveJaguarCombat != null
        )
        {
            lockHorizontal = forms.ActiveJaguarCombat.IsCharging;
        }

        motor.Move(input.Horizontal, lockHorizontal);
    }

    void UpdateDirectionBools()
    {
        Animator a = forms.ActiveAnimator;
        if (a == null)
        {
            return;
        }

        float h = input.Horizontal;
        a.SetBool("Izq", h < -0.01f);
        a.SetBool("Der", h > 0.01f);
    }
}