using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Spawns the player onto the graph and moves between linked nodes from WASD input.
public class CharacterMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private CharacterInputController inputController;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    private Node currentNode;
    private Node targetNode;
    private bool moveUpWasPressed;
    private bool moveDownWasPressed;
    private bool moveLeftWasPressed;
    private bool moveRightWasPressed;

    private void Awake()
    {
        if (inputController == null)
        {
            inputController = GetComponent<CharacterInputController>();
        }
    }

    private void Start()
    {
        SpawnAtRandomPlayerNode();
    }

    private void Update()
    {
        if (inputController == null || currentNode == null)
        {
            return;
        }

        MoveTowardTargetNode();

        if (targetNode != null)
        {
            CacheInputState();
            return;
        }

        HandleMovementInput();
        CacheInputState();
    }

    // Finds a random node marked as a player spawn and places the player on it.
    private void SpawnAtRandomPlayerNode()
    {
        Node[] allNodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
        List<Node> spawnNodes = new List<Node>();

        foreach (Node node in allNodes)
        {
            if (node.PlayerSpawns)
            {
                spawnNodes.Add(node);
            }
        }

        if (spawnNodes.Count == 0)
        {
            Debug.LogWarning("CharacterMovement could not find any nodes marked with PlayerSpawns.");
            return;
        }

        Node spawnNode = spawnNodes[UnityEngine.Random.Range(0, spawnNodes.Count)];
        SnapToNode(spawnNode);
    }

    // Moves once for the first newly pressed direction this frame.
    private void HandleMovementInput()
    {
        if (inputController.MoveUpPressed && !moveUpWasPressed)
        {
            TryMoveToNeighbor(Node.NeighborDirection.Up);
            return;
        }

        if (inputController.MoveDownPressed && !moveDownWasPressed)
        {
            TryMoveToNeighbor(Node.NeighborDirection.Down);
            return;
        }

        if (inputController.MoveLeftPressed && !moveLeftWasPressed)
        {
            TryMoveToNeighbor(Node.NeighborDirection.Left);
            return;
        }

        if (inputController.MoveRightPressed && !moveRightWasPressed)
        {
            TryMoveToNeighbor(Node.NeighborDirection.Right);
        }
    }

    // Attempts to move to the node connected in the requested direction.
    private void TryMoveToNeighbor(Node.NeighborDirection direction)
    {
        Node nextNode = currentNode.GetNeighbor(direction);

        if (nextNode == null)
        {
            return;
        }

        targetNode = nextNode;
    }

    // Moves the player toward the current target node at a constant speed.
    private void MoveTowardTargetNode()
    {
        if (targetNode == null)
        {
            return;
        }

        Vector3 targetPosition = new Vector3(targetNode.Center.x, targetNode.Center.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            currentNode = targetNode;
            targetNode = null;
        }
    }

    // Snaps the player to a node immediately and remembers it as the current node.
    private void SnapToNode(Node node)
    {
        currentNode = node;
        targetNode = null;
        transform.position = new Vector3(node.Center.x, node.Center.y, transform.position.z);
    }

    // Stores the current input state so movement only happens on a fresh key press.
    private void CacheInputState()
    {
        moveUpWasPressed = inputController.MoveUpPressed;
        moveDownWasPressed = inputController.MoveDownPressed;
        moveLeftWasPressed = inputController.MoveLeftPressed;
        moveRightWasPressed = inputController.MoveRightPressed;
    }
}
