using System.Collections;
using UnityEngine;

public class BoulderController : MonoBehaviour
{
    private Transform target;
    private Rigidbody2D rb;
    private float damage; // Damage the boulder will deal
    public float gravityScale = 1f; // Gravity applied to the boulder
    [SerializeField] private float launchSpeed = 15f; // Initial speed for a high arc
    [SerializeField] private float launchAngle = 75f; // Angle to create a steep arc
    [SerializeField] private float homingStrength = 0.1f; // Strength of homing effect
    [SerializeField] private float explosionRadius = 3f; // Radius for AoE damage
    [SerializeField] private Animator animator; // Animator for playing the breaking animation

    private bool isHomingActive = false;
    public bool rotateTowardsVelocity = false;
    public bool burnDot = false;
    public bool slowProjectile = false;
    public int slowLevel;
    
    private Collider2D coll;

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) {
            Debug.LogError("Rigidbody2D component missing from boulder. Please add Rigidbody2D to the boulder prefab.");
            return;
        }

        if (animator == null) {
            animator = GetComponent<Animator>();
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
        this.damage = damage; // Set the boulder's damage

        if (target != null) {
            if (rb != null) {
                LaunchProjectileInArc();
            } else {
                Debug.LogWarning("Boulder has no Rigidbody2D.");
            }
        } else {
            Debug.LogWarning("Boulder has no target.");
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

        //Debug.Log($"Boulder launched with initial velocity: {initialVelocity} towards target at {target.position}");
    }

    private void Update()
    {
        if (target == null) {
            Destroy(gameObject);
            //Debug.Log("target is null");
            return;
        }

        if (!isHomingActive && rb.velocity.y <= 0) {
            isHomingActive = true;
        }

        if (isHomingActive) {
            ApplyHomingEffect();
        }

        if (rotateTowardsVelocity) {
            RotateTowardsVelocity();
        }
    }

    private void ApplyHomingEffect()
    {
        coll.enabled = true;
        Vector2 directionToTarget = (target.position - transform.position).normalized;
        Vector2 desiredVelocity = directionToTarget * rb.velocity.magnitude;
        rb.velocity = Vector2.Lerp(rb.velocity, desiredVelocity, homingStrength * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            // Trigger the breaking animation
            if (animator != null) {
                StartCoroutine(AnimationDelay());
            }

            // Apply AoE damage
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Enemy"));
            foreach (Collider2D enemyCollider in hitEnemies) {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null) {
                    if (burnDot) {
                        float dotDuration = 3f; // Duration of the DoT effect
                        float damagePerSecond = 5f; // Damage per second applied

                        // Apply a DoT effect to the enemy
                        enemy.ApplyDoTWithVisuals(damagePerSecond, dotDuration, Enemy.DamageType.Fire, Color.red);
                        enemy.TakeDamage(damage, Enemy.DamageType.Fire); // Call the TakeDamage method on each enemy
                    } else {
                        enemy.TakeDamage(damage, Enemy.DamageType.Normal); // Call the TakeDamage method on each enemy
                    }
                    if (slowProjectile) {
                        enemy.ApplySlowEffect((0.5f * slowLevel), 3f);
                    }
                }
            }

            // Destroy the boulder after a short delay to allow the animation to play
            StartCoroutine(DestroyAfterAnimation());
        }
    }

    private IEnumerator AnimationDelay()
    {
        yield return new WaitForSeconds(0.125f);
        animator.SetTrigger("Break");
        rb.velocity = Vector2.zero;
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(0.5f); // Adjust delay as needed based on animation length
        //Debug.Log("Destroying object.");
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw explosion radius in the editor for visualization
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    private void RotateTowardsVelocity()
    {
        Vector2 velocity = rb.velocity;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
