using UnityEngine;

[CreateAssetMenu(fileName = "New Waypoint Path", menuName = "Waypoints/Waypoint Path")]
public class WaypointPath : ScriptableObject
{
    public string pathName; // Identifier for the path
    public Vector3[] waypoints; // Positions of each waypoint in the path
}
