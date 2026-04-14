using UnityEngine;

// A node is one point in the graph and can connect to up to four neighbors.
[ExecuteAlways]
public class Node : MonoBehaviour
{
    [Header("Node Position")]
    [SerializeField] private Vector2 center;

    public Vector2 Center => center;
    public Node Up { get; private set; }
    public Node Down { get; private set; }
    public Node Left { get; private set; }
    public Node Right { get; private set; }

    private void Awake()
    {
        SyncCenterWithTransform();
    }

    private void OnValidate()
    {
        SyncCenterWithTransform();
    }

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

    public void SetUp(Node node)
    {
        Up = node;
    }

    public void SetDown(Node node)
    {
        Down = node;
    }

    public void SetLeft(Node node)
    {
        Left = node;
    }

    public void SetRight(Node node)
    {
        Right = node;
    }
}
