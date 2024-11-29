using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuUpgradePanel : MonoBehaviour
{
    public PlayerUpgradeData playerUpgradeData; // Reference to the ScriptableObject
    public TMP_Text damageUpgradeText;
    public TMP_Text rangeUpgradeText;
    public TMP_Text fireRateUpgradeText;

    public Button increaseDamageButton;
    public Button increaseRangeButton;
    public Button increaseFireRateButton;

    private string selectedTowerType = "basicArchers"; // The type of tower to upgrade

    private void Start()
    {
        UpdateUI();
        increaseDamageButton.onClick.AddListener(() => UpgradeStat("damage"));
        increaseRangeButton.onClick.AddListener(() => UpgradeStat("range"));
        increaseFireRateButton.onClick.AddListener(() => UpgradeStat("fireRate"));
    }

    private void UpgradeStat(string statType)
    {
        // Retrieve or create upgrade entry for the selected tower type
        TowerUpgrades towerUpgrade = playerUpgradeData.GetUpgradesForTower(selectedTowerType);

        if (towerUpgrade == null) {
            // Create a new upgrade entry if it doesn't exist
            var newEntry = new TowerUpgradeEntry { towerName = selectedTowerType, upgrades = new TowerUpgrades() };
            playerUpgradeData.towerUpgrades.Add(newEntry);
            towerUpgrade = newEntry.upgrades;
        }

        // Apply the upgrade based on the statType
        switch (statType) {
            case "damage":
                towerUpgrade.additionalDamage += 1; // Example increment
                break;
            case "range":
                towerUpgrade.additionalRange += 1; // Example increment
                break;
            case "fireRate":
                towerUpgrade.additionalFireRate -= 0.1f; // Example increment
                break;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        TowerUpgrades towerUpgrade = playerUpgradeData.GetUpgradesForTower(selectedTowerType);

        if (towerUpgrade != null) {
            int damageUpgradePercent = towerUpgrade.additionalDamage * 100;
            int fireRateUpgradePercent = Mathf.RoundToInt(towerUpgrade.additionalFireRate * 100); // Convert to percent

            damageUpgradeText.text = $"Damage Upgrade: +{towerUpgrade.additionalDamage}";
            rangeUpgradeText.text = $"Range Upgrade: +{towerUpgrade.additionalRange}";
            fireRateUpgradeText.text = $"Fire Rate Upgrade: {fireRateUpgradePercent}%";
        }

    }
}
