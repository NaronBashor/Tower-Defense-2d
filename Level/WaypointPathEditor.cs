using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaypointPath))]
public class WaypointPathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw the default inspector elements

        WaypointPath waypointPath = (WaypointPath)target;

        // Add a button in the Inspector to populate waypoints
        if (GUILayout.Button("Populate Waypoints from Selected Transforms")) {
            PopulateWaypointsFromSelection(waypointPath);
        }
    }

    private void PopulateWaypointsFromSelection(WaypointPath waypointPath)
    {
        // Get all selected transforms in the scene
        Transform[] selectedTransforms = Selection.transforms;

        // If there are no selected transforms, show a warning
        if (selectedTransforms.Length == 0) {
            Debug.LogWarning("No transforms selected! Please select the waypoint objects in the scene.");
            return;
        }

        // Populate the waypoints array with the positions of selected transforms
        waypointPath.waypoints = new Vector3[selectedTransforms.Length];
        for (int i = 0; i < selectedTransforms.Length; i++) {
            waypointPath.waypoints[i] = selectedTransforms[i].position;
        }

        // Mark the ScriptableObject as dirty so changes are saved
        EditorUtility.SetDirty(waypointPath);

        //Debug.Log($"Populated {waypointPath.waypoints.Length} waypoints from selected objects.");
    }
}
