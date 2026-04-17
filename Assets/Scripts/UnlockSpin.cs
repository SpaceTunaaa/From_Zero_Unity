using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class UnlockSpin : MonoBehaviour
{
    private Collider2D triggerCollider;
    private bool unlocked;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;
    }

    private void OnValidate()
    {
        Collider2D collider2D = GetComponent<Collider2D>();

        if (collider2D != null)
        {
            collider2D.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (unlocked)
        {
            return;
        }

        Rigidbody2D playerBody = other.attachedRigidbody;
        GameObject playerObject = playerBody != null ? playerBody.gameObject : other.gameObject;

        CharacterJumpRotation jumpRotation = playerObject.GetComponent<CharacterJumpRotation>();

        if (jumpRotation == null)
        {
            return;
        }

        jumpRotation.enabled = true;
        unlocked = true;
        Destroy(gameObject);
    }
}
