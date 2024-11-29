using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanelController : MonoBehaviour
{
    public GameObject upgradePanel;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI fireRateText;
    public TextMeshProUGUI incomeText; // New text element for displaying income

    // UI elements for next level preview
    public TextMeshProUGUI nextDamageText;
    public TextMeshProUGUI nextRangeText;
    public TextMeshProUGUI nextFireRateText;
    public TextMeshProUGUI nextIncomeText; // New text element for next level income

    public Button upgradeButton;
    public Button sellButton;
    public Button closeButton;

    private TowerController currentTower;

    private void Start()
    {
        upgradePanel.SetActive(false);
    }

    public void Initialize(TowerController tower)
    {
        //upgradePanel.SetActive(true);
        currentTower = tower;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentTower.IsIncomeTower()) {
            // Show and update income-specific UI
            incomeText.gameObject.SetActive(true);
            incomeText.text = $"Income: {currentTower.GetGoldPerSecond()} Gold/sec";

            if (currentTower.HasNextLevel()) {
                Tower nextLevelData = currentTower.GetNextLevelData();
                nextIncomeText.gameObject.SetActive(true);
                nextIncomeText.text = $"Income: {nextLevelData.goldPerSecond} Gold/sec";
            } else {
                nextIncomeText.gameObject.SetActive(false);
                nextIncomeText.text = "Max Level";
            }

            // Hide non-relevant stats for income towers
            damageText.text = "N/A";
            rangeText.text = "N/A";
            fireRateText.text = "N/A";
            nextDamageText.text = "";
            nextRangeText.text = "";
            nextFireRateText.text = "";
        } else {
            // Show stats for non-income towers
            damageText.text = $"Damage: {currentTower.Damage}";
            rangeText.text = $"Range: {currentTower.Range}";
            fireRateText.text = $"Fire Rate: {currentTower.FireRate.ToString("F2")}";

            if (currentTower.HasNextLevel()) {
                Tower nextLevelData = currentTower.GetNextLevelData();
                TowerUpgrades upgrades = currentTower.playerUpgradeData.GetUpgradesForTower(currentTower.towerType);

                int nextDamage = nextLevelData.damage + (upgrades?.additionalDamage ?? 0);
                int nextRange = nextLevelData.range + (int)(upgrades?.additionalRange ?? 0);
                float nextFireRate = nextLevelData.fireRate + (upgrades?.additionalFireRate ?? 0);

                nextDamageText.text = $"Damage: {nextDamage}";
                nextRangeText.text = $"Range: {nextRange}";
                nextFireRateText.text = $"Fire Rate: {nextFireRate.ToString("F2")}";
            } else {
                nextDamageText.text = "";
                nextRangeText.text = "Max Level";
                nextFireRateText.text = "";
            }

            // Hide income stats for non-income towers
            incomeText.gameObject.SetActive(false);
            nextIncomeText.gameObject.SetActive(false);
        }
    }


    public void UpgradeTower()
    {
        //Debug.Log("Upgrading Tower.");
        if (currentTower.Upgrade()) {
            UpdateUI(); // Refresh UI if upgrade was successful
        }
    }

    public void SellTower()
    {
        if (currentTower != null) {
            Destroy(currentTower.gameObject); // Destroy the tower on sell
        }
        CloseMenu();
    }

    public void CloseMenu()
    {
        upgradePanel.SetActive(false);
    }

    public void OpenMenu()
    {
        upgradePanel.SetActive(true);
    }

    // New method to update the current tower reference after an upgrade
    public void SetCurrentTower(TowerController newTower)
    {
        currentTower = newTower;
        Initialize(newTower); // Reinitialize with the new tower instance
    }
}
