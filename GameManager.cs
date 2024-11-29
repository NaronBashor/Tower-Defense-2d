using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public PlayerUpgradeData playerUpgradeData; // Reference to the PlayerUpgradeData ScriptableObject
    public List<GameObject> activeTowers = new List<GameObject>(); // Track all active towers

    public void NewGame()
    {
        ResetPlayerUpgrades();
        ResetTowers();
        ResetGameProgress();

        Debug.Log("New game started, all stats and towers reset.");
    }

    private void ResetPlayerUpgrades()
    {
        foreach (var upgrade in playerUpgradeData.towerUpgrades) {
            upgrade.upgrades.additionalDamage = 0;
            upgrade.upgrades.additionalRange = 0;
            upgrade.upgrades.additionalFireRate = 0;
        }
        Debug.Log("Player upgrades reset to default.");
    }

    private void ResetTowers()
    {
        foreach (GameObject tower in activeTowers) {
            if (tower != null) {
                Destroy(tower); // Destroy each tower GameObject
            }
        }
        activeTowers.Clear(); // Clear the list of active towers
        Debug.Log("All towers have been removed from the game.");
    }

    private void ResetGameProgress()
    {
        // Here you can reset any other game-related data, such as player resources, levels, etc.
        // For example:
        // playerResources = startingResources;
        // playerScore = 0;
        Debug.Log("Game progress reset.");
    }

    // Method to register new towers so they can be reset later
    public void RegisterTower(GameObject tower)
    {
        activeTowers.Add(tower);
    }
}
