using UnityEngine;
using UnityEngine.InputSystem;

// Reads player movement input from the generated input-action map.
public class CharacterInputController : MonoBehaviour, Custom_Input_Action_Map.IPlayerActions
{
    // Runtime instance of the project's input actions.
    private Custom_Input_Action_Map input;

    // Direction key states that other scripts can read.
    public bool MoveUpPressed { get; private set; }
    public bool MoveDownPressed { get; private set; }
    public bool MoveLeftPressed { get; private set; }
    public bool MoveRightPressed { get; private set; }
    public bool ExpandPressed { get; private set; }

    // Combines the four direction buttons into a single 2D movement vector.
    public Vector2 MoveDirection => new Vector2(
        (MoveRightPressed ? 1f : 0f) - (MoveLeftPressed ? 1f : 0f),
        (MoveUpPressed ? 1f : 0f) - (MoveDownPressed ? 1f : 0f)
    );

    // Creates and enables the input map when this component becomes active.
    private void OnEnable()
    {
        if (input == null)
        {
            input = new Custom_Input_Action_Map();
        }

        input.Player.SetCallbacks(this);
        input.Player.Enable();
    }

    // Disables input and clears cached button states when this component stops.
    private void OnDisable()
    {
        if (input == null)
        {
            return;
        }

        input.Player.Disable();
        input.Player.RemoveCallbacks(this);

        MoveUpPressed = false;
        MoveDownPressed = false;
        MoveLeftPressed = false;
        MoveRightPressed = false;
        ExpandPressed = false;
    }

    // Updates a movement key state and logs the moment it is first pressed.
    private bool UpdateMoveState(bool currentState, InputAction.CallbackContext context, string action)
    {
        bool newState = context.ReadValueAsButton();

        if (!currentState && newState)
        {
            Debug.Log($"{action}");
        }

        return newState;
    }

    // Updates the "up" state from the W key action.
    public void OnMoveUp(InputAction.CallbackContext context)
    {
        MoveUpPressed = UpdateMoveState(MoveUpPressed, context, "MoveUp");
    }

    // Updates the "down" state from the S key action.
    public void OnMoveDown(InputAction.CallbackContext context)
    {
        MoveDownPressed = UpdateMoveState(MoveDownPressed, context, "MoveDown");
    }

    // Updates the "left" state from the A key action.
    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        MoveLeftPressed = UpdateMoveState(MoveLeftPressed, context, "MoveLeft");
    }

    // Updates the "right" state from the D key action.
    public void OnMoveRight(InputAction.CallbackContext context)
    {
        MoveRightPressed = UpdateMoveState(MoveRightPressed, context, "MoveRight");
    }

    // Updates the expand state from the any-key action.
    public void OnExpand(InputAction.CallbackContext context)
    {
        ExpandPressed = UpdateMoveState(ExpandPressed, context, "Expand");
    }
}
