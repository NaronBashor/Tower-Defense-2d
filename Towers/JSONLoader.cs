using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerDataWrapper
{
    public TowerData data;
}


public class JSONLoader : MonoBehaviour
{
    public static JSONLoader Instance { get; private set; }
    public TowerData towerData;

    private void Awake()
    {
        // Singleton setup to ensure only one instance of JSONLoader
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadTowerData();
        AssignPrefabsToTowers();
    }

    private void LoadTowerData()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("Data/tower_data");
        if (jsonData == null) {
            Debug.LogError("JSON file not found in Resources/Data folder!");
            return;
        }

        try {
            TowerDataWrapper wrapper = JsonUtility.FromJson<TowerDataWrapper>(jsonData.text);
            towerData = wrapper.data;

            if (towerData != null) {
                //Debug.Log("Tower data loaded successfully!");
            } else {
                Debug.LogError("Failed to load tower data.");
            }
        } catch (System.Exception e) {
            Debug.LogError("Error parsing JSON: " + e.Message);
        }
    }


    private void AssignPrefabsToTowers()
    {
        // For each tower category, load the corresponding prefab from Resources
        if (towerData != null) {
            AssignPrefabs(towerData.incomeTowers);
            AssignPrefabs(towerData.rangeTowers);
            AssignPrefabs(towerData.damageBoostTowers);
            AssignPrefabs(towerData.attackSpeedTowers);
            AssignPrefabs(towerData.basicArchers);
            AssignPrefabs(towerData.poisonArchers);
            AssignPrefabs(towerData.explosiveArchers);
            AssignPrefabs(towerData.piercingArchers);
            AssignPrefabs(towerData.fireMages);
            AssignPrefabs(towerData.iceMages);
            AssignPrefabs(towerData.lightningMages);
            AssignPrefabs(towerData.arcaneMages);
            AssignPrefabs(towerData.bluntStones);
            AssignPrefabs(towerData.stunningStones);
            AssignPrefabs(towerData.fireStones);
            AssignPrefabs(towerData.heavyStones);
        }
    }

    private void AssignPrefabs(Tower[] towers)
    {
        if (towers == null) return;

        foreach (var tower in towers) {
            LoadPrefabForTower(tower);
        }
    }

    private void LoadPrefabForTower(Tower tower)
    {
        // Construct the path for the prefab in the Resources folder
        string prefabPath = $"Prefabs/Towers/{tower.name}";

        // Load the prefab
        tower.towerPrefab = Resources.Load<GameObject>(prefabPath);

        if (tower.towerPrefab == null) {
            Debug.LogWarning($"Prefab not found for tower '{tower.name}' at path: {prefabPath}");
        }
    }
}
