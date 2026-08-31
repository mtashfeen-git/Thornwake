using UnityEngine;

public class TouchInputManager : MonoBehaviour
{
    public static TouchInputManager instance;

    public bool leftHeld;
    public bool rightHeld;
    public bool jumpPressed;
    public bool jumpReleased;
    public bool attackPressed;

    private void Awake()
    {
        instance = this;
    }

    // --- Movement (held) ---
    public void OnLeftDown() => leftHeld = true;
    public void OnLeftUp() => leftHeld = false;

    public void OnRightDown() => rightHeld = true;
    public void OnRightUp() => rightHeld = false;

    // --- Jump (press + release) ---
    public void OnJumpDown() => jumpPressed = true;
    public void OnJumpUp() => jumpReleased = true;

    // --- Attack (single press) ---
    public void OnAttackDown() => attackPressed = true;

    public void ConsumeJump() => jumpPressed = false;
    public void ConsumeJumpReleased() => jumpReleased = false;
    public void ConsumeAttack() => attackPressed = false;

    public float GetHorizontal()
    {
        if (leftHeld) return -1f;
        if (rightHeld) return 1f;
        return 0f;
    }
}