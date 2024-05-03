using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    /// <summary>
    /// Implement interface from InputAction(Controls)/IPlayerActions
    /// InputAction(Controls)/IPlayerActions use to make buttons by override method from InputActions
    /// </summary>

    public event Action<Vector2> MoveEvent;
    public event Action<bool> PrimaryFireEvent;

    public Vector2 AimPosition { get; private set; }

    private Controls controls;

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }

        controls.Player.Enable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
        if (context.performed) // .performed is pressed
        {
            // ?. to check if null
            PrimaryFireEvent?.Invoke(true);
        }
        else if (context.canceled) // .canceled is not pressed
        {
            // ?. to check if null
            PrimaryFireEvent?.Invoke(false);
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        AimPosition = context.ReadValue<Vector2>();
    }
}