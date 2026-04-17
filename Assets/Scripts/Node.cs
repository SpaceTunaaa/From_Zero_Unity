using UnityEngine;

// A node is one point in the graph and can connect to up to four neighbors.
[ExecuteAlways]
public class Node : MonoBehaviour
{
    // The four directions this node can connect to.
    public enum NeighborDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    // How far each scene-view connection stub extends from the node center.
    private const float ConnectionStubLength = 0.75f;

    // Stores the node's 2D world position based on the GameObject pivot.
    [Header("Node Position")]
    [SerializeField] private Vector2 center;

    // Marks whether this node should be used as a player spawn point.
    [Header("Node Settings")]
    [SerializeField] private bool playerSpawns;
    [SerializeField] private bool isTransition;

    // Inspector-editable references to neighboring nodes.
    [Header("Neighbor Links")]
    [SerializeField] private Node up;
    [SerializeField] private Node down;
    [SerializeField] private Node left;
    [SerializeField] private Node right;

    // Read-only accessors for the node position and each linked neighbor.
    public Vector2 Center => center;
    public bool PlayerSpawns => playerSpawns;
    public bool IsTransition => isTransition;
    public Node Up => up;
    public Node Down => down;
    public Node Left => left;
    public Node Right => right;

    // Sync the stored center as soon as the component wakes up.
    private void Awake()
    {
        SyncCenterWithTransform();
    }

    // Keep the stored center updated when values change in the Inspector.
    private void OnValidate()
    {
        SyncCenterWithTransform();
    }

    // Keep the center synced while the object is moved in edit mode or play mode.
    private void Update()
    {
        SyncCenterWithTransform();
    }

    // Keeps the stored 2D center equal to this object's world-space pivot.
    private void SyncCenterWithTransform()
    {
        center = transform.position;
    }

    // Moves the node in world space and updates the stored center.
    public void SetCenter(Vector2 newCenter)
    {
        center = newCenter;
        transform.position = new Vector3(newCenter.x, newCenter.y, transform.position.z);
    }

    // Returns the neighbor linked in the requested direction.
    public Node GetNeighbor(NeighborDirection direction)
    {
        return direction switch
        {
            NeighborDirection.Up => up,
            NeighborDirection.Down => down,
            NeighborDirection.Left => left,
            NeighborDirection.Right => right,
            _ => null
        };
    }

    // Assigns a neighbor link in the requested direction.
    public void SetNeighbor(NeighborDirection direction, Node node)
    {
        switch (direction)
        {
            case NeighborDirection.Up:
                up = node;
                break;
            case NeighborDirection.Down:
                down = node;
                break;
            case NeighborDirection.Left:
                left = node;
                break;
            case NeighborDirection.Right:
                right = node;
                break;
        }
    }

    // Returns the world-space point where a direction handle/line should begin.
    public Vector3 GetConnectionPoint(NeighborDirection direction)
    {
        return transform.position + (Vector3)(GetDirectionVector(direction) * ConnectionStubLength);
    }

    // Converts a direction enum into a unit vector.
    public static Vector2 GetDirectionVector(NeighborDirection direction)
    {
        return direction switch
        {
            NeighborDirection.Up => Vector2.up,
            NeighborDirection.Down => Vector2.down,
            NeighborDirection.Left => Vector2.left,
            NeighborDirection.Right => Vector2.right,
            _ => Vector2.zero
        };
    }

    // Draws the node center plus the four directional connection stubs when selected.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.12f);

        DrawConnectionGizmo(NeighborDirection.Up, up, Color.cyan);
        DrawConnectionGizmo(NeighborDirection.Down, down, Color.magenta);
        DrawConnectionGizmo(NeighborDirection.Left, left, Color.green);
        DrawConnectionGizmo(NeighborDirection.Right, right, Color.red);
    }

    // Draws one direction stub and, if present, a line to the linked neighbor.
    private void DrawConnectionGizmo(NeighborDirection direction, Node neighbor, Color color)
    {
        Vector3 start = transform.position;
        Vector3 stubEnd = GetConnectionPoint(direction);

        Gizmos.color = color;
        Gizmos.DrawLine(start, stubEnd);
        Gizmos.DrawSphere(stubEnd, 0.08f);

        if (neighbor != null)
        {
            Gizmos.DrawLine(stubEnd, neighbor.transform.position);
        }
    }
}
