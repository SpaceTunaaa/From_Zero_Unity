using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class CharacterJumpRotation : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private Transform visualTarget;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private bool rotateClockwiseWhenMovingRight = true;
    [SerializeField] private bool snapUprightOnLanding = true;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayers = ~0;
    [SerializeField] private float groundCheckDepth = 0.08f;

    private Rigidbody2D body;
    private BoxCollider2D bodyCollider;
    private bool wasGrounded;
    private int rotationDirection = -1;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();

        if (visualTarget == null)
        {
            visualTarget = CreateRuntimeVisualTarget();
        }
    }

    private void Start()
    {
        wasGrounded = IsGrounded();
        SnapToLandingRotation();
    }

    private void Update()
    {
        bool grounded = IsGrounded();

        if (wasGrounded && !grounded)
        {
            SetRotationDirectionFromMovement();
        }

        if (grounded)
        {
            if (!wasGrounded)
            {
                SnapToLandingRotation();
            }
        }
        else
        {
            visualTarget.Rotate(0f, 0f, rotationDirection * rotationSpeed * Time.deltaTime);
        }

        wasGrounded = grounded;
    }

    private void SetRotationDirectionFromMovement()
    {
        float horizontalVelocity = body.linearVelocity.x;
        int movementDirection = horizontalVelocity < -0.01f ? -1 : 1;
        rotationDirection = rotateClockwiseWhenMovingRight ? -movementDirection : movementDirection;
    }

    private void SnapToLandingRotation()
    {
        float targetZRotation = 0f;

        if (!snapUprightOnLanding)
        {
            targetZRotation = Mathf.Round(visualTarget.localEulerAngles.z / 90f) * 90f;
        }

        visualTarget.localRotation = Quaternion.Euler(0f, 0f, targetZRotation);
    }

    private bool IsGrounded()
    {
        Bounds bounds = bodyCollider.bounds;
        Vector2 checkCenter = new Vector2(bounds.center.x, bounds.min.y - groundCheckDepth * 0.5f);
        Vector2 checkSize = new Vector2(bounds.size.x * 0.9f, groundCheckDepth);
        Collider2D[] hits = Physics2D.OverlapBoxAll(checkCenter, checkSize, 0f, groundLayers);

        foreach (Collider2D hit in hits)
        {
            if (hit != null && hit != bodyCollider && !hit.isTrigger)
            {
                return true;
            }
        }

        return false;
    }

    private Transform CreateRuntimeVisualTarget()
    {
        SpriteRenderer rootRenderer = GetComponent<SpriteRenderer>();

        if (rootRenderer == null)
        {
            return transform;
        }

        GameObject visualObject = new GameObject("PlayerVisual");
        Transform visualTransform = visualObject.transform;
        visualTransform.SetParent(transform, false);

        SpriteRenderer visualRenderer = visualObject.AddComponent<SpriteRenderer>();
        visualRenderer.sprite = rootRenderer.sprite;
        visualRenderer.color = rootRenderer.color;
        visualRenderer.flipX = rootRenderer.flipX;
        visualRenderer.flipY = rootRenderer.flipY;
        visualRenderer.drawMode = rootRenderer.drawMode;
        visualRenderer.size = rootRenderer.size;
        visualRenderer.tileMode = rootRenderer.tileMode;
        visualRenderer.maskInteraction = rootRenderer.maskInteraction;
        visualRenderer.sortingLayerID = rootRenderer.sortingLayerID;
        visualRenderer.sortingOrder = rootRenderer.sortingOrder;
        visualRenderer.spriteSortPoint = rootRenderer.spriteSortPoint;
        visualRenderer.sharedMaterials = rootRenderer.sharedMaterials;

        rootRenderer.enabled = false;
        return visualTransform;
    }
}
