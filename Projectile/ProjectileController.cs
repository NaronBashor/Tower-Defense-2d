using System.Collections;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private Transform target;
    private Rigidbody2D rb;
    private float damage; // Damage the projectile will deal
    public float gravityScale = 1f; // Gravity applied to the projectile
    [SerializeField] private float launchSpeed = 15f; // Initial speed for a high arc
    [SerializeField] private float launchAngle = 75f; // Angle to create a steep arc
    [SerializeField] private float homingStrength = 0.1f; // Strength of homing effect

    private bool isHomingActive = false;
    private Collider2D coll;

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) {
            Debug.LogError("Rigidbody2D component missing from projectile. Please add Rigidbody2D to the projectile prefab.");
            return;
        }

        coll.enabled = false;
    }

    private void Start()
    {
        if (rb != null) {
            rb.gravityScale = gravityScale;
        }
    }

    // Initialize method with target and damage
    public void Initialize(Transform target, float damage)
    {
        this.target = target;
        this.damage = damage; // Set the projectile's damage

        if (target != null) {
            if (rb != null) {
                LaunchProjectileInArc();
            } else {
                Debug.LogWarning("Projectile has no Rigidbody2D.");
            }
        } else {
            Debug.LogWarning("Projectile has no target.");
        }
    }

    private void LaunchProjectileInArc()
    {
        Vector2 directionToTarget = (target.position - transform.position).normalized;
        float angleInRadians = launchAngle * Mathf.Deg2Rad;
        float vx = Mathf.Cos(angleInRadians) * launchSpeed;
        float vy = Mathf.Sin(angleInRadians) * launchSpeed;

        Vector2 initialVelocity = new Vector2(vx * directionToTarget.x, vy);
        rb.velocity = initialVelocity;

        //Debug.Log($"Projectile launched with initial velocity: {initialVelocity} towards target at {target.position}");
    }

    private void Update()
    {
        if (target == null) {
            Destroy(gameObject);
            return;
        }

        if (!isHomingActive && rb.velocity.y <= 0) {
            isHomingActive = true;
            coll.enabled = true;
        }

        if (isHomingActive) {
            ApplyHomingEffect();
        }

        RotateTowardsVelocity();
    }

    private void ApplyHomingEffect()
    {
        Vector2 directionToTarget = (target.position - transform.position).normalized;
        Vector2 desiredVelocity = directionToTarget * rb.velocity.magnitude;
        rb.velocity = Vector2.Lerp(rb.velocity, desiredVelocity, homingStrength * Time.deltaTime);
    }

    private void RotateTowardsVelocity()
    {
        Vector2 velocity = rb.velocity;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) {
            // Apply damage to the enemy (assuming the enemy has a method to receive damage)
            Enemy enemy = collision.GetComponent<Enemy>();
            StartCoroutine(Delay(enemy));
        }
    }

    private IEnumerator Delay(Enemy enemy)
    {
        yield return new WaitForSeconds(0.0625f);

        if (enemy != null) {
            enemy.TakeDamage(damage, Enemy.DamageType.Normal); // Call the TakeDamage method on the enemy
            Destroy(gameObject); // Destroy the projectile on hit
        }
    }
}
