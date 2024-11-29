using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public string appearance;
    public string lore;
    public float health;
    public float speed;
    public int damage;
    public string special;
    public bool isKnockbackResistant;
    public float dodgeChance;
    public float fireResistance;
    public float damageResistance;
    public bool isDamageResistant;

    // Tower Debuffs
    public float debuffRange = 5f;
    public float towerDamageReduction;

    // Rage mode properties
    public float rageHealthThreshold;
    public float rageSpeedMultiplier;
    public float rageDamageMultiplier;

    // Status effect immunity
    public bool isImmuneToStatusEffects = false;

    // Phase Shift properties
    public bool canPhaseShift = false; // Enable or disable phase shift
    public float teleportRange; // Maximum range of teleportation
    public float phaseShiftCooldown; // Time between possible teleports
    public float phaseShiftChance; // 20% chance to teleport each check
    public int phaseShiftSkipWaypoints = 2; // Number of waypoints to skip when phase shifting

    // Invisibility properties
    public bool canBecomeInvisible = false; // Toggle for invisibility ability
    public float invisibilityDuration = 2f; // Time spent invisible
    public float invisibilityCooldown = 10f; // Time between invisibility activations
}
