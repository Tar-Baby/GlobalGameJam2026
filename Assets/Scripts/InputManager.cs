using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float Horizontal { get; private set; }

    public bool JumpPressed { get; private set; }

    public bool ActionPressed { get; private set; }
    public bool ActionHeld { get; private set; }
    public bool ActionReleased { get; private set; }

    public bool AbilityLeftPressed { get; private set; }
    public bool AbilityRightPressed { get; private set; }

    void Update()
    {
        ReadMovement();
        ReadActions();
    }

    void ReadMovement()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");
        JumpPressed = Input.GetButtonDown("Jump");
    }

    void ReadActions()
    {
        ActionPressed = Input.GetButtonDown("Action");
        ActionHeld = Input.GetButton("Action");
        ActionReleased = Input.GetButtonUp("Action");

        AbilityLeftPressed = Input.GetButtonDown("AbilityLeft");
        AbilityRightPressed = Input.GetButtonDown("AbilityRight");
    }
}