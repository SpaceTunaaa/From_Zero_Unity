using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Character2DInputController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 60f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField, Range(0f, 1f)] private float jumpCutMultiplier = 0.5f;
    [SerializeField, Range(0f, 1f)] private float airControl = 0.6f;
    [SerializeField] private float coyoteTime = 0.12f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayers = ~0;
    [SerializeField] private float groundCheckDepth = 0.08f;

    [Header("Wall Jump")]
    [SerializeField] private LayerMask wallLayers = ~0;
    [SerializeField] private float wallCheckDistance = 0.08f;
    [SerializeField] private Vector2 wallJumpForce = new Vector2(7f, 7f);
    [SerializeField] private float wallJumpControlLockTime = 0.12f;

    private Rigidbody2D body;
    private BoxCollider2D bodyCollider;
    private float horizontalInput;
    private bool jumpQueued;
    private bool jumpReleaseQueued;
    private float coyoteTimeCounter;
    private float wallJumpControlLockCounter;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();
        body.constraints |= RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        ReadKeyboardInput();
    }

    private void FixedUpdate()
    {
        Vector2 velocity = body.linearVelocity;
        bool grounded = IsGrounded();
        int wallDirection = GetWallDirection();
        coyoteTimeCounter = grounded ? coyoteTime : coyoteTimeCounter - Time.fixedDeltaTime;

        if (wallDirection != 0)
        {
            coyoteTimeCounter = coyoteTime;
        }

        wallJumpControlLockCounter -= Time.fixedDeltaTime;

        if (wallJumpControlLockCounter <= 0f)
        {
            float control = grounded ? 1f : airControl;
            float targetSpeed = horizontalInput * moveSpeed;
            velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, acceleration * control * Time.fixedDeltaTime);
        }

        if (jumpQueued && coyoteTimeCounter > 0f)
        {
            if (!grounded && wallDirection != 0)
            {
                velocity.x = -wallDirection * wallJumpForce.x;
                velocity.y = wallJumpForce.y;
                wallJumpControlLockCounter = wallJumpControlLockTime;
            }
            else
            {
                velocity.y = jumpForce;
            }

            coyoteTimeCounter = 0f;
        }

        if (jumpReleaseQueued && velocity.y > 0f)
        {
            velocity.y *= jumpCutMultiplier;
        }

        body.linearVelocity = velocity;
        jumpQueued = false;
        jumpReleaseQueued = false;
    }

    private void ReadKeyboardInput()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            horizontalInput = 0f;
            return;
        }

        horizontalInput = 0f;

        if (keyboard.aKey.isPressed)
        {
            horizontalInput -= 1f;
        }

        if (keyboard.dKey.isPressed)
        {
            horizontalInput += 1f;
        }

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            jumpQueued = true;
        }

        if (keyboard.spaceKey.wasReleasedThisFrame)
        {
            jumpReleaseQueued = true;
        }
    }

    private bool IsGrounded()
    {
        Bounds bounds = bodyCollider.bounds;
        Vector2 checkCenter = new Vector2(bounds.center.x, bounds.min.y - groundCheckDepth * 0.5f);
        Vector2 checkSize = new Vector2(bounds.size.x * 0.9f, groundCheckDepth);

        return HasSolidColliderInBox(checkCenter, checkSize, groundLayers);
    }

    private int GetWallDirection()
    {
        Bounds bounds = bodyCollider.bounds;
        Vector2 checkSize = new Vector2(wallCheckDistance, bounds.size.y * 0.85f);
        Vector2 leftCheckCenter = new Vector2(bounds.min.x - wallCheckDistance * 0.5f, bounds.center.y);
        Vector2 rightCheckCenter = new Vector2(bounds.max.x + wallCheckDistance * 0.5f, bounds.center.y);

        if (HasSolidColliderInBox(leftCheckCenter, checkSize, wallLayers))
        {
            return -1;
        }

        if (HasSolidColliderInBox(rightCheckCenter, checkSize, wallLayers))
        {
            return 1;
        }

        return 0;
    }

    private bool HasSolidColliderInBox(Vector2 center, Vector2 size, LayerMask layerMask)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, layerMask);

        foreach (Collider2D hit in hits)
        {
            if (hit != null && hit != bodyCollider && !hit.isTrigger)
            {
                return true;
            }
        }

        return false;
    }
}
