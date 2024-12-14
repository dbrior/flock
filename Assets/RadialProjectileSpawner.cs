using UnityEngine;
using System.Collections;

public class RadialProjectileSpawner : MonoBehaviour
{
    [Header("Projectile Spawning Settings")]
    [Tooltip("Prefab of the projectile to be spawned")]
    public GameObject projectilePrefab;

    [Tooltip("Number of projectiles to spawn in a single burst")]
    [Range(1, 36)]
    public int projectileCount = 8;

    [Tooltip("Speed of each spawned projectile")]
    public float projectileMoveSpeed = 5f;

    [Tooltip("Damage value for each projectile")]
    public float projectileDamage = 10f;

    [Tooltip("Knockback force for each projectile")]
    public float projectileKnockbackForce = 5f;

    [Tooltip("Radius from which projectiles are spawned")]
    public float spawnRadius = 1f;

    [Tooltip("Interval between attack bursts in seconds")]
    public float attackIntervalSec = 2f;

    [Header("Rotation Settings")]
    [Tooltip("Rotation speed in degrees per second")]
    public float rotationSpeed = 90f;

    [Tooltip("Rotation direction")]
    public bool rotateClockwise = true;

    [Tooltip("Angle offset for initial rotation")]
    public float angleOffset = 0f;

    [Tooltip("Randomize the spawn angle slightly")]
    public bool randomizeAngle = false;

    private float currentRotation = 0f;
    private float lastAttackTime;

    private void Start()
    {
        // Ensure first attack happens soon after spawning
        lastAttackTime = -attackIntervalSec;
        StartCoroutine("Randomization");
    }

    private void Update()
    {
        // Rotate the spawner
        float rotationDirection = rotateClockwise ? -1f : 1f;
        currentRotation += rotationSpeed * rotationDirection * Time.deltaTime;

        // Check if it's time for the next attack
        if (Time.time >= lastAttackTime + attackIntervalSec)
        {
            SpawnProjectilesBurst();
            lastAttackTime = Time.time;
        }
    }

    IEnumerator Randomization() {
        while (true) {
            rotationSpeed = Random.Range(0f, 30f);
            attackIntervalSec = Random.Range(0.4f, 1f);
            angleOffset = Random.Range(0, 90f);
            // projectileCount = Random.Range(3, 15);
            yield return new WaitForSeconds(Random.Range(1f,4f));
        }
    }

    /// <summary>
    /// Spawns projectiles in a radial pattern around the object
    /// </summary>
    private void SpawnProjectilesBurst()
    {
        // Calculate the angle between each projectile
        float angleStep = 360f / projectileCount;

        for (int i = 0; i < projectileCount; i++)
        {
            // Calculate the base angle
            float angle = i * angleStep + angleOffset + currentRotation;

            // Optional: Add slight randomization to the angle
            if (randomizeAngle)
            {
                angle += Random.Range(-15f, 15f);
            }

            // Convert angle to radians for vector calculation
            float radians = angle * Mathf.Deg2Rad;

            // Calculate the initial heading (tangential to the radial direction)
            Vector2 initialHeading = new Vector2(
                Mathf.Cos(radians), 
                Mathf.Sin(radians)
            );

            // Spawn the projectile at an offset from the current position
            Vector2 spawnPosition = (Vector2)transform.position + initialHeading * spawnRadius;
            GameObject projectileInstance = Instantiate(projectilePrefab, spawnPosition, Quaternion.Euler(0,0,angle));

            // Get the Projectile component and configure it
            Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                // Rotate the initial heading by the projectile's rotation
                Vector2 rotatedHeading = RotateVector2(initialHeading, -angle);
                
                projectileScript.SetHeading(rotatedHeading);
                projectileScript.moveSpeed = projectileMoveSpeed;
                projectileScript.damage = projectileDamage;
                projectileScript.knockbackForce = projectileKnockbackForce;
                projectileScript.SetOwner(GetComponent<Collider2D>());
                projectileScript.isKinematic = true;
            }

            Destroy(projectileInstance, 5f);
        }
    }

    /// <summary>
    /// Rotate a 2D vector by a given angle in degrees
    /// </summary>
    private Vector2 RotateVector2(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        
        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
    }

    /// <summary>
    /// Visualize spawn radius in the scene view
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Draw a wire circle to show the spawn radius in the Unity Editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}