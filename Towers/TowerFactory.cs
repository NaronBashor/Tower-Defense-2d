using UnityEngine;

public class TowerFactory : MonoBehaviour
{
    public GameObject SpawnTower(string category, int index, Vector3 position)
    {
        Tower[] towerLevels = GetTowerLevels(category);
        if (towerLevels == null || towerLevels.Length == 0) {
            Debug.LogError("No levels found for this tower type.");
            return null; // Return null if no tower levels are found
        }

        GameObject towerInstance = Instantiate(towerLevels[index].towerPrefab, position, Quaternion.identity);
        towerInstance.name = towerLevels[index].name;

        TowerController controller = towerInstance.GetComponent<TowerController>();
        if (controller != null) {
            controller.Initialize(towerLevels);
        } else {
            Debug.LogError("TowerController component not found on the instantiated tower prefab.");
        }

        return towerInstance; // Return the instantiated tower
    }

    private Tower[] GetTowerLevels(string category)
    {
        TowerData towerData = JSONLoader.Instance.towerData;

        return category switch {
            "incomeTowers" => towerData.incomeTowers,
            "rangeTowers" => towerData.rangeTowers,
            "damageBoostTowers" => towerData.damageBoostTowers,
            "attackSpeedTowers" => towerData.attackSpeedTowers,
            "basicArchers" => towerData.basicArchers,
            "poisonArchers" => towerData.poisonArchers,
            "explosiveArchers" => towerData.explosiveArchers,
            "piercingArchers" => towerData.piercingArchers,
            "fireMages" => towerData.fireMages,
            "iceMages" => towerData.iceMages,
            "lightningMages" => towerData.lightningMages,
            "arcaneMages" => towerData.arcaneMages,
            "bluntStones" => towerData.bluntStones,
            "stunningStones" => towerData.stunningStones,
            "fireStones" => towerData.fireStones,
            "heavyStones" => towerData.heavyStones,
            _ => null,
        };
    }
}
