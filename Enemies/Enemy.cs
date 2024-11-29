using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine.UI;
using System.Linq;

public class Enemy : MonoBehaviour
{
    // TODO: Move to tower/projectile script
    public enum DamageType
    {
        Normal,
        Fire,
        Ice,
        // Add more types as needed
    }

    // TODO: Move to tower/projectile script
    public enum StatusEffect
    {
        Poison,
        Slow,
        // Add more status effects as needed
    }

    public GameObject healthBarPrefab; // Assign the prefab in the Inspector
    private GameObject healthBarInstance;
    private Image healthBarFill; // Reference to the foreground image fill

    public GameObject floatingTextPrefab;
    public EnemyData enemyData; // Reference to the ScriptableObject with stats
    private Animator animator;
    private Renderer enemyRenderer;
    private Collider enemyCollider;

    private float health;
    private float speed;
    private int damage;
    private bool isInRageMode = false;

    private Vector3[] waypoints;
    private int currentWaypointIndex = 0;

    private bool isTakingDoT = false; // Track if DoT is active

    //private List<Tower> debuffedTowers = new List<Tower>(); // Track towers that have been debuffed

    public float Health { get { return health; } set { health = value; } }

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (enemyData != null) {
            InitializeEnemy();
        } else {
            Debug.LogError("EnemyData is not assigned!");
        }

        // Each enemy gets a random path from LevelManager
        waypoints = LevelManager.Instance.GetRandomPathWaypoints();

        if (waypoints == null || waypoints.Length == 0) {
            Debug.LogError("No waypoints found for the current level!");
        }

        if (enemyData.canPhaseShift) {
            StartCoroutine(PhaseShiftRoutine());
        }

        if (enemyData.canBecomeInvisible) {
            StartCoroutine(InvisibilityRoutine());
        }

        // Start applying tower debuff when the enemy is active
        //StartCoroutine(ApplyTowerDebuff());

        // Instantiate the health bar and set it as a child of the enemy
        if (healthBarPrefab != null) {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity, transform);
            healthBarFill = healthBarInstance.transform.Find("Background/Fill").GetComponent<Image>(); // Adjust based on your prefab hierarchy
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null) {
            healthBarFill.fillAmount = health / enemyData.health;
        }
    }


    private void InitializeEnemy()
    {
        health = enemyData.health;
        speed = enemyData.speed / 3.5f;
        damage = enemyData.damage;
    }

    private void Update()
    {
        if (currentWaypointIndex < waypoints.Length) {
            MoveAlongPath();
        }
    }

    private void MoveAlongPath()
    {
        if (waypoints.Length == 0) return;

        Vector3 targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = targetWaypoint - transform.position;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, targetWaypoint) < 0.1f) {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length) {
                ReachEndOfPath();
            }
        }
    }

    private void ReachEndOfPath()
    {
        Destroy(gameObject);
    }

    // New method to calculate path progress as a normalized value between 0 and 1
    public float GetPathProgress()
    {
        if (waypoints == null || waypoints.Length == 0) {
            return 0f;
        }

        // Calculate normalized progress based on current waypoint index
        float progress = (float)currentWaypointIndex / (waypoints.Length - 1);
        return Mathf.Clamp01(progress); // Clamp to ensure value stays between 0 and 1
    }

    public void ApplyDoTWithVisuals(float damagePerSecond, float duration, DamageType damageType, Color textColor)
    {
        StartCoroutine(DamageOverTimeWithVisualsRoutine(damagePerSecond, duration, damageType, textColor));
    }

    private IEnumerator DamageOverTimeWithVisualsRoutine(float damagePerSecond, float duration, DamageType damageType, Color textColor)
    {
        float elapsed = 0f;
        float interval = 0.5f; // Interval between damage ticks (adjust as needed)

        while (elapsed < duration) {
            TakeDamage(damagePerSecond * interval, damageType);
            ShowFloatingText(damagePerSecond * interval, textColor); // Show red-colored text for each DoT tick
            elapsed += interval;
            yield return new WaitForSeconds(interval); // Wait for the next tick
        }
    }


    public void TakeDamage(float amount, DamageType damageType)
    {
        if (CanDodge()) {
            Debug.Log($"{enemyData.enemyName} dodged the attack!");
            return;
        }

        amount = ApplyResistance(amount, damageType);

        if (damageType == DamageType.Fire) {
            ShowFloatingText(amount, Color.red);
        } else if (damageType == DamageType.Normal) {
            ShowFloatingText(amount, Color.black);
        }

        health -= amount;

        UpdateHealthBar();

        //Debug.Log($"{enemyData.enemyName} took {amount} {damageType} damage. Remaining Health: {health}");

        CheckRageMode();

        if (health <= 0) {
            //RemoveTowerDebuff();
            Die();
        }
    }

    private void ShowFloatingText(float damageAmount, Color color)
    {
        if (floatingTextPrefab != null) {
            // Instantiate the floating text slightly above the enemy position
            GameObject floatingTextInstance = Instantiate(floatingTextPrefab, transform.position + new Vector3(-0.2f, 0.4f), Quaternion.identity);
            FloatingDamageText floatingText = floatingTextInstance.GetComponent<FloatingDamageText>();
            if (floatingText != null) {
                floatingText.SetText(damageAmount.ToString(), color);
            }
        }
    }

    private void Die()
    {
        animator.SetTrigger("DieTrigger");
        Destroy(gameObject, 1f);
    }

    // Other existing methods...

    private bool CanDodge()
    {
        float dodgeRoll = Random.value;
        return dodgeRoll < enemyData.dodgeChance;
    }

    public void ApplySlowEffect(float slowAmount, float duration)
    {
        StartCoroutine(SlowEffectRoutine(slowAmount, duration));
    }

    private IEnumerator SlowEffectRoutine(float slowAmount, float duration)
    {
        float originalSpeed = speed; // Store the original speed
        speed *= (1 - slowAmount); // Reduce speed by the specified percentage (e.g., 0.5 for 50% reduction)

        // Show feedback (e.g., change color or play particle effect if needed)
        Debug.Log($"{enemyData.enemyName} is slowed by {slowAmount * 100}% for {duration} seconds.");

        yield return new WaitForSeconds(duration); // Wait for the effect duration

        speed = originalSpeed; // Reset speed back to normal
        Debug.Log($"{enemyData.enemyName}'s slow effect has worn off.");
    }


    private float ApplyResistance(float amount, DamageType damageType)
    {
        switch (damageType) {
            case DamageType.Fire:
                amount *= (1 - enemyData.fireResistance);
                break;
        }
        if (enemyData.isDamageResistant) {
            amount *= (1 - enemyData.damageResistance);
        }
        return amount;
    }

    private void CheckRageMode()
    {
        if (!isInRageMode && health <= enemyData.health * enemyData.rageHealthThreshold) {
            ActivateRageMode();
        }
    }

    private void ActivateRageMode()
    {
        isInRageMode = true;
        speed *= enemyData.rageSpeedMultiplier;
        damage = Mathf.RoundToInt(damage * enemyData.rageDamageMultiplier);
        animator.SetTrigger("RageTrigger");
        //Debug.Log($"{enemyData.enemyName} has entered rage mode!");
    }

    private IEnumerator PhaseShiftRoutine()
    {
        while (health > 0) {
            yield return new WaitForSeconds(enemyData.phaseShiftCooldown);

            if (Random.value < enemyData.phaseShiftChance) {
                PhaseShift();
            }
        }
    }

    private void PhaseShift()
    {
        int waypointsToSkip = enemyData.phaseShiftSkipWaypoints;
        currentWaypointIndex += waypointsToSkip;
        if (currentWaypointIndex >= waypoints.Length) {
            currentWaypointIndex = waypoints.Length - 1;
        }
        transform.position = waypoints[currentWaypointIndex];
        animator.SetTrigger("TeleportTrigger");
    }

    private IEnumerator InvisibilityRoutine()
    {
        while (health > 0) {
            yield return new WaitForSeconds(enemyData.invisibilityCooldown);

            BecomeInvisible();
            yield return new WaitForSeconds(enemyData.invisibilityDuration);

            BecomeVisible();
        }
    }

    private void BecomeInvisible()
    {
        enemyRenderer.enabled = false;
        enemyCollider.enabled = false;
        Debug.Log($"{enemyData.enemyName} has become invisible!");
    }

    private void BecomeVisible()
    {
        enemyRenderer.enabled = true;
        enemyCollider.enabled = true;
        Debug.Log($"{enemyData.enemyName} is visible again!");
    }

    //private IEnumerator ApplyTowerDebuff()
    //{
    //    while (health > 0) {
    //        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, enemyData.debuffRange, LayerMask.GetMask("Tower"));
    //        List<Tower> towersInRange = new List<Tower>();

    //        foreach (var hitCollider in hitColliders) {
    //            Tower tower = hitCollider.GetComponent<Tower>();
    //            if (tower != null) {
    //                towersInRange.Add(tower);
    //                Debug.Log($"Detected tower: {tower.name}");

    //                if (!debuffedTowers.Contains(tower)) {
    //                    tower.ApplyDamageDebuff(enemyData.towerDamageReduction);
    //                    debuffedTowers.Add(tower);
    //                    Debug.Log($"{enemyData.enemyName} debuffed {tower.name} by {enemyData.towerDamageReduction * 100}%");
    //                }
    //            }
    //        }

    //        for (int i = debuffedTowers.Count - 1; i >= 0; i--) {
    //            if (!towersInRange.Contains(debuffedTowers[i])) {
    //                debuffedTowers[i].RemoveDamageDebuff(enemyData.towerDamageReduction);
    //                Debug.Log($"{enemyData.enemyName} removed debuff from {debuffedTowers[i].name}");
    //                debuffedTowers.RemoveAt(i);
    //            }
    //        }

    //        yield return new WaitForSeconds(1f);
    //    }
    //}





    //private void RemoveTowerDebuff()
    //{
    //    // Remove debuff from all towers in range
    //    foreach (Tower tower in debuffedTowers) {
    //        tower.RemoveDamageDebuff(enemyData.towerDamageReduction);
    //    }
    //    debuffedTowers.Clear();
    //}

    //private void OnDrawGizmosSelected()
    //{
    //    if (enemyData != null) {
    //        Gizmos.color = new Color(1, 0, 0, 0.4f); // Semi-transparent red
    //        Gizmos.DrawWireSphere(transform.position, enemyData.debuffRange);
    //    }
    //}

}
