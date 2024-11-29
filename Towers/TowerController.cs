using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tower;

public class TowerController : MonoBehaviour
{
    public PlayerUpgradeData playerUpgradeData; // Reference the ScriptableObject
    public string towerType; // Unique identifier for this tower's type

    [SerializeField] private Transform center;
    [SerializeField] private Transform projectilePos;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] List<Animator> animators = new List<Animator>();
    [SerializeField] private UpgradePanelController upgradePanel;

    private int currentLevelIndex = 0;
    private Tower[] towerLevels;
    private string towerName;
    private int damage;
    private int range;
    private float initialFireRate;
    private float fireRate;
    private TargetingStrategy targetingStrategy;
    private TowerAnimator towerAnimator;
    private bool isIncomeTower = false;

    private TowerSelectionUI towerSelectionUI; // Reference to TowerSelectionUI

    private Coroutine incomeCoroutine; // To manage income generation coroutine
    public int slowLevel;

    public int Damage { get { return damage; } }
    public int Range { get { return range; } }
    public float FireRate { get { return initialFireRate; } }

    private void Awake()
    {
        upgradePanel = FindAnyObjectByType<UpgradePanelController>();
    }

    private void Start()
    {
        towerSelectionUI = FindObjectOfType<TowerSelectionUI>();

        towerAnimator = GetComponentInChildren<TowerAnimator>();
        if (towerAnimator == null) {
            Debug.LogWarning("TowerAnimator component not found in this GameObject or its children.");
        } else {
            animators = new List<Animator>(GetComponentsInChildren<Animator>());
            towerAnimator.SetAnimators(animators);
        }
    }

    public void Initialize(Tower[] towerLevelsData)
    {
        towerLevels = towerLevelsData;

        if (towerLevelsData == null || towerLevelsData.Length == 0) {
            //Debug.LogError("Initialize() called with null or empty towerLevelsData.");
            return;
        }

        towerLevels = towerLevelsData;
        //Debug.Log($"Initialized towerLevels with {towerLevels.Length} entries.");

        InitializeTowerWithUpgrades(); // Ensures that the tower applies initial data
        isIncomeTower = towerLevels[currentLevelIndex].isIncomeTower;

        //Debug.Log($"{towerType} initialized. IsIncomeTower: {isIncomeTower}");

        if (isIncomeTower) {
            StartIncomeGeneration();
        }
    }

    public bool IsIncomeTower()
    {
        return isIncomeTower;
    }

    public float GetGoldPerSecond()
    {
        return towerLevels[currentLevelIndex].goldPerSecond;
    }

    private void StartIncomeGeneration()
    {
        if (incomeCoroutine != null) {
            StopCoroutine(incomeCoroutine);
        }

        incomeCoroutine = StartCoroutine(GenerateIncome());
    }

    private IEnumerator GenerateIncome()
    {
        Tower currentTower = towerLevels[currentLevelIndex];
        //Debug.Log("Current Level Index: " + currentLevelIndex);
        if (currentTower.isIncomeTower) {
            while (true) {
                if (GoldManager.Instance != null) {
                    GoldManager.Instance.EarnGold(currentTower.goldPerSecond);
                    //Debug.Log($"{towerName} generated {currentTower.goldPerSecond} gold.");
                }
                yield return new WaitForSeconds(1f); // Generate gold every second
            }
        }
    }

    private void InitializeTowerWithUpgrades()
    {
        // Apply initial level's base stats with main menu upgrades
        ApplyTowerData(towerLevels[currentLevelIndex]); // Initialize with upgrades applied once

        //Debug.Log($"Initial spawn for {towerType} with upgrades: Damage={damage}, Range={range}, FireRate={fireRate}");
    }


    private void ApplyTowerData(Tower towerData)
    {
        if (playerUpgradeData == null) {
            //Debug.LogError("PlayerUpgradeData is not assigned in TowerController.");
            return;
        }

        // Retrieve the upgrades for the current tower type
        TowerUpgrades upgrades = playerUpgradeData.GetUpgradesForTower(towerType);

        // Set base stats for the current level
        towerName = towerData.name;

        if (!towerData.isIncomeTower) {
            // Update damage, range, and fire rate only for non-income towers
            damage = towerData.damage + (upgrades?.additionalDamage ?? 0);
            range = towerData.range + (int)(upgrades?.additionalRange ?? 0);
            initialFireRate = towerData.fireRate + (upgrades?.additionalFireRate ?? 0);
            fireRate = initialFireRate; // Reset fire rate to the upgraded value
            //Debug.Log($"Upgrades applied for {towerType} at level {currentLevelIndex}: Damage={damage}, Range={range}, FireRate={initialFireRate}");
        } else {
            // Only log or handle income-specific properties if necessary
            //Debug.Log($"Applied income-specific data for {towerType} at level {currentLevelIndex}: GoldPerSecond={towerData.goldPerSecond}");
        }

        targetingStrategy = towerData.targetingStrategy;
    }




    public bool Upgrade()
    {
        if (currentLevelIndex >= towerLevels.Length - 1) {
            return false; // Already at max level
        }

        // Move to the next level
        currentLevelIndex++;
        ApplyTowerData(towerLevels[currentLevelIndex]);

        // Check if the tower remains an income tower and update behavior
        isIncomeTower = towerLevels[currentLevelIndex].isIncomeTower;
        if (isIncomeTower) {
            StartIncomeGeneration(); // Restart income generation with updated values
        } else if (incomeCoroutine != null) {
            StopCoroutine(incomeCoroutine);
            incomeCoroutine = null; // Stop income generation if upgraded to a non-income tower
        }

        ReplaceTowerPrefab();
        return true;
    }





    private void Update()
    {
        fireRate -= Time.deltaTime;
        if (fireRate <= 0) {
            Attack();
            //Debug.Log("Attacked.");
            fireRate = initialFireRate;
        }
    }

    public bool HasNextLevel()
    {
        // Check if there is a next level
        return currentLevelIndex < towerLevels.Length - 1;
    }

    public Tower GetNextLevelData()
    {
        if (HasNextLevel()) {
            // Return the data for the next level
            return towerLevels[currentLevelIndex + 1];
        }

        return null; // No next level
    }

    private void Attack()
    {
        GameObject target = FindTarget();
        if (target != null) {
            //Debug.Log("Found a target.");
            towerAnimator?.PlayAttackAnimation();
            SpawnProjectile(target);
        }
        if (animators.Count > 1) {
            StartCoroutine(SecondProjectileDelay());
        }
    }

    private IEnumerator SecondProjectileDelay()
    {
        yield return new WaitForSeconds(0.125f);
        GameObject target = FindTarget();
        if (target != null) {
            //Debug.Log("Found a target.");
            towerAnimator?.PlayAttackAnimation();
            SpawnProjectile(target);
        }
    }

    private GameObject FindTarget()
    {
        if (center == null) {
            //Debug.LogError("Center Transform not assigned in TowerController. Please assign a center for range detection.");
            return null;
        }

        // Use Physics2D.OverlapCircle for 2D colliders
        Collider2D[] hits = Physics2D.OverlapCircleAll(center.position, range, LayerMask.GetMask("Enemy"));
        //Debug.Log($"Checking for enemies within range: {range} at position {center.position}. Hits found: {hits.Length}");

        List<GameObject> enemiesInRange = new List<GameObject>();

        foreach (var hit in hits) {
            if (hit != null) {
                //Debug.Log($"Detected enemy: {hit.gameObject.name}");
                enemiesInRange.Add(hit.gameObject);
            }
        }

        if (enemiesInRange.Count == 0) {
            //Debug.Log("No enemies found within range.");
            return null;
        }

        switch (targetingStrategy) {
            case TargetingStrategy.FirstTarget:
                return GetFirstTarget(enemiesInRange);
            case TargetingStrategy.MostHealth:
                return GetMostHealthTarget(enemiesInRange);
            default:
                return enemiesInRange[0];
        }
    }



    private GameObject GetFirstTarget(List<GameObject> enemies)
    {
        GameObject firstTarget = null;
        float furthestProgress = float.MinValue;

        foreach (var enemy in enemies) {
            Enemy enemyController = enemy.GetComponent<Enemy>();
            if (enemyController != null) {
                float progress = enemyController.GetPathProgress();
                if (progress > furthestProgress) {
                    furthestProgress = progress;
                    firstTarget = enemy;
                }
            }
        }
        return firstTarget;
    }

    private GameObject GetMostHealthTarget(List<GameObject> enemies)
    {
        GameObject mostHealthTarget = null;
        float maxHealth = float.MinValue;

        foreach (var enemy in enemies) {
            Enemy enemyController = enemy.GetComponent<Enemy>();
            if (enemyController != null && enemyController.Health > maxHealth) {
                maxHealth = enemyController.Health;
                mostHealthTarget = enemy;
            }
        }
        return mostHealthTarget;
    }

    private void SpawnProjectile(GameObject target)
    {
        if (projectilePrefab == null) return;

        GameObject projectileInstance = Instantiate(projectilePrefab, projectilePos.position, Quaternion.identity);
        ProjectileController projectileController = projectileInstance.GetComponent<ProjectileController>();

        if (projectileController != null) {
            float damage = this.damage; // Assume `this.damage` is the damage value from the tower's data
            projectileController.Initialize(target.transform, damage); // Pass damage along with the target
        } else {
            BoulderController boulderController = projectileInstance.GetComponent<BoulderController>();
            float damage = this.damage; // Assume `this.damage` is the damage value from the tower's data
            boulderController.Initialize(target.transform, damage); // Pass damage along with the target
            if (towerType == "stunningStones") {
                boulderController.slowLevel = currentLevelIndex + 1;
            }
        }
    }


    public void SetTargetingStrategy(TargetingStrategy newStrategy)
    {
        targetingStrategy = newStrategy;
    }

    private void ReplaceTowerPrefab()
    {
        GameObject newPrefab = towerLevels[currentLevelIndex].towerPrefab;

        if (newPrefab != null && newPrefab != this.gameObject) {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;

            GameObject newTowerInstance = Instantiate(newPrefab, position, rotation);
            TowerController newController = newTowerInstance.GetComponent<TowerController>();
            if (newController != null) {
                // Pass the current level index and towerLevels to the new instance
                newController.towerLevels = this.towerLevels; // Ensure this is public or has a setter
                newController.SetLevelIndex(currentLevelIndex);

                // Initialize the new tower
                newController.Initialize(towerLevels);

                if (upgradePanel != null) {
                    upgradePanel.SetCurrentTower(newController);
                }
            }

            //Debug.Log("Scheduled destruction of old tower instance.");
            StartCoroutine(DestroyOldTowerWithDelay());
        }
    }





    private IEnumerator DestroyOldTowerWithDelay()
    {
        yield return new WaitForEndOfFrame();
        Destroy(this.gameObject);
    }





    public void SetLevelIndex(int levelIndex)
    {
        if (towerLevels == null) {
            Debug.LogError("towerLevels is null in SetLevelIndex.");
            return;
        }

        if (towerLevels.Length <= levelIndex) {
            Debug.LogError($"Invalid levelIndex {levelIndex}. towerLevels length: {towerLevels.Length}");
            return;
        }

        currentLevelIndex = levelIndex;
        if (towerLevels[currentLevelIndex] == null) {
            Debug.LogError("towerLevels[currentLevelIndex] is null in SetLevelIndex.");
            return;
        }

        ApplyTowerData(towerLevels[currentLevelIndex]);
    }


    // Draw a gizmo to represent the tower's range when selected
    private void OnDrawGizmosSelected()
    {
        // Set the color for the gizmo (optional)
        //Gizmos.color = Color.red;

        // Draw a wire sphere at the tower's position with the specified range as the radius
        //Gizmos.DrawWireSphere(center.position, range);

        // Draw filled sphere if game is running to visualize detection range
        if (Application.isPlaying) {
            Gizmos.color = new Color(1, 0, 0, 0.2f); // Semi-transparent red
            Gizmos.DrawSphere(center.position, range);
        }
    }

    private void OnMouseDown()
    {
        // Check if a tower is currently being placed
        if (towerSelectionUI != null && towerSelectionUI.IsPlacingTower) {
            Debug.Log("Cannot open upgrade panel while placing a tower.");
            return; // Prevent interaction with the upgrade panel if a tower is being placed
        }

        upgradePanel.OpenMenu();
        upgradePanel.Initialize(this);
    }

    private void OnDestroy()
    {
        if (incomeCoroutine != null) {
            StopCoroutine(incomeCoroutine);
        }
    }
}
