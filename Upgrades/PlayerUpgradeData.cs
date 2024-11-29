using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerUpgradeData", menuName = "Game/PlayerUpgradeData")]
public class PlayerUpgradeData : ScriptableObject
{
    public List<TowerUpgradeEntry> towerUpgrades = new List<TowerUpgradeEntry>();

    // Fetch upgrades for a specific tower type by name
    public TowerUpgrades GetUpgradesForTower(string towerType)
    {
        foreach (var entry in towerUpgrades) {
            if (entry.towerName == towerType) {
                return entry.upgrades;
            }
        }
        return null; // Return null if no upgrades exist for this type
    }
}

[System.Serializable]
public class TowerUpgradeEntry
{
    public string towerName; // The type name, e.g., "basicArchers"
    public TowerUpgrades upgrades; // The upgrade values for this type
}

[System.Serializable]
public class TowerUpgrades
{
    public int additionalDamage;
    public float additionalRange;
    public float additionalFireRate;
}
