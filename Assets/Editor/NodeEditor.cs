using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Node))]
public class NodeEditor : Editor
{
    private static Node activeSourceNode;
    private static Node.NeighborDirection? activeDirection;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Node node = (Node)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Scene Linking", EditorStyles.boldLabel);

        if (activeSourceNode == node && activeDirection.HasValue)
        {
            EditorGUILayout.HelpBox(
                $"Click another node in the Scene view to assign the {activeDirection.Value} neighbor.",
                MessageType.Info
            );

            if (GUILayout.Button("Cancel Linking"))
            {
                ClearActiveLink();
            }
        }
        else
        {
            EditorGUILayout.HelpBox(
                "Use the colored direction handles in the Scene view to connect this node to another node.",
                MessageType.None
            );
        }
    }

    private void OnSceneGUI()
    {
        Node node = (Node)target;

        DrawDirectionHandle(node, Node.NeighborDirection.Up, Color.cyan);
        DrawDirectionHandle(node, Node.NeighborDirection.Down, Color.magenta);
        DrawDirectionHandle(node, Node.NeighborDirection.Left, Color.green);
        DrawDirectionHandle(node, Node.NeighborDirection.Right, Color.red);

        if (activeSourceNode == node && activeDirection.HasValue)
        {
            HandleSceneLinking(node, activeDirection.Value);
        }
    }

    private void DrawDirectionHandle(Node node, Node.NeighborDirection direction, Color color)
    {
        Vector3 center = node.transform.position;
        Vector3 handlePosition = node.GetConnectionPoint(direction);
        float handleSize = HandleUtility.GetHandleSize(handlePosition) * 0.12f;

        using (new Handles.DrawingScope(color))
        {
            Handles.DrawLine(center, handlePosition);

            if (Handles.Button(handlePosition, Quaternion.identity, handleSize, handleSize, Handles.SphereHandleCap))
            {
                activeSourceNode = node;
                activeDirection = direction;
                SceneView.RepaintAll();
            }
        }
    }

    private void HandleSceneLinking(Node node, Node.NeighborDirection direction)
    {
        Event currentEvent = Event.current;
        Vector3 handlePosition = node.GetConnectionPoint(direction);

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        using (new Handles.DrawingScope(Color.white))
        {
            Vector3 mouseWorld = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition).origin;
            mouseWorld.z = node.transform.position.z;
            Handles.DrawDottedLine(handlePosition, mouseWorld, 4f);
        }

        if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.Escape)
        {
            ClearActiveLink();
            currentEvent.Use();
            return;
        }

        if (currentEvent.type != EventType.MouseDown || currentEvent.button != 0)
        {
            return;
        }

        GameObject clickedObject = HandleUtility.PickGameObject(currentEvent.mousePosition, false);
        Node targetNode = clickedObject != null ? clickedObject.GetComponent<Node>() : null;

        if (targetNode != null && targetNode != node)
        {
            Undo.RecordObject(node, $"Set {direction} neighbor");
            node.SetNeighbor(direction, targetNode);
            EditorUtility.SetDirty(node);
        }

        ClearActiveLink();
        currentEvent.Use();
    }

    private static void ClearActiveLink()
    {
        activeSourceNode = null;
        activeDirection = null;
        SceneView.RepaintAll();
    }
}
