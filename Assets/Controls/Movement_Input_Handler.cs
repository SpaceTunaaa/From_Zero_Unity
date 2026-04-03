using UnityEngine;
using UnityEngine.InputSystem;

public class Movement_Input_Handler : MonoBehaviour
{
    public Player_Movement input; // reference the script we generated

    private void OnEnable()
   {
        // initialize input
       input = new Player_Movement();
       input.Player.Enable();
   }


   private void OnDisable()
   {
       input.Player.Disable(); // making sure that disabled object does not take inputs
   }

    private void Update()
   {
       Vector2 move = input.Player.Movement.ReadValue<Vector2>();
       if (move != Vector2.zero)
       {
           transform.position += new Vector3(move.x, move.y, 0) * 0.01f;
       }
   }

}
