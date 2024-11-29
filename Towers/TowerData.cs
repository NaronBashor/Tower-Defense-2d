using System;
using UnityEngine;

[System.Serializable]
public class Tower
{
    public enum TargetingStrategy
    {
        FirstTarget,
        MostHealth
    }

    public int level;
    public string name;
    public int damage;
    public int range;
    public float fireRate;
    public GameObject towerPrefab;
    public TargetingStrategy targetingStrategy;

    // New properties for income towers
    public bool isIncomeTower;
    public float goldPerSecond;

    private int originalDamage;
    private int currentDamage;

    // Constructor to initialize original damage
    public Tower(int damage)
    {
        originalDamage = currentDamage = damage;
    }

    public void Initialize()
    {
        // Ensure the current damage matches the original on initialization
        originalDamage = currentDamage = damage;
    }

    public void ApplyDamageDebuff(float reductionPercentage)
    {
        if (reductionPercentage > 0 && currentDamage == originalDamage) {
            currentDamage = Mathf.RoundToInt(currentDamage * (1 - reductionPercentage));
            Debug.Log($"{name} damage reduced by {reductionPercentage * 100}%");
        }
    }

    public void RemoveDamageDebuff(float reductionPercentage)
    {
        if (reductionPercentage > 0 && currentDamage < originalDamage) {
            currentDamage = originalDamage; // Restore to the original damage value
            Debug.Log($"{name} damage debuff removed.");
        }
    }

    public int GetCurrentDamage()
    {
        return currentDamage;
    }
}

[System.Serializable]
public class TowerData
{
    public Tower[] incomeTowers;
    public Tower[] rangeTowers;
    public Tower[] damageBoostTowers;
    public Tower[] attackSpeedTowers;
    public Tower[] basicArchers;
    public Tower[] poisonArchers;
    public Tower[] explosiveArchers;
    public Tower[] piercingArchers;
    public Tower[] fireMages;
    public Tower[] iceMages;
    public Tower[] lightningMages;
    public Tower[] arcaneMages;
    public Tower[] bluntStones;
    public Tower[] stunningStones;
    public Tower[] fireStones;
    public Tower[] heavyStones;
}
