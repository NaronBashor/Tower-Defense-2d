using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LevelPaths
{
    public List<WaypointPath> paths; // List of waypoint paths for a specific level
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance; // Singleton pattern for easy access

    public int currentLevel = 1; // The current level
    public GameObject[] levelBackgrounds; // Array of background images
    public List<LevelPaths> levelPaths; // List of level paths, each containing multiple waypoint paths

    private GameObject activeBackground;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes if needed
        } else {
            Destroy(gameObject);
        }

        LoadLevel(currentLevel); // Load the initial level
    }

    public void LoadLevel(int level)
    {
        if (level < 1 || level > levelBackgrounds.Length || level > levelPaths.Count) {
            Debug.LogError("Level out of range!");
            return;
        }

        currentLevel = level;

        // Disable the previous background if one is active
        if (activeBackground != null) {
            activeBackground.SetActive(false);
        }

        // Set the new background for the level
        activeBackground = levelBackgrounds[level - 1];
        activeBackground.SetActive(true);

        //Debug.Log($"Loaded background for level {level}");
    }

    // New method to get a random path for the current level
    public Vector3[] GetRandomPathWaypoints()
    {
        List<WaypointPath> paths = levelPaths[currentLevel - 1].paths;
        if (paths == null || paths.Count == 0) {
            Debug.LogError("No paths found for level " + currentLevel);
            return null;
        }

        // Select a random path from the available paths for this level
        int randomPathIndex = Random.Range(0, paths.Count);
        WaypointPath selectedPath = paths[randomPathIndex];
        //Debug.Log($"Selected random path {selectedPath.pathName} for enemy");

        return selectedPath.waypoints;
    }
}
