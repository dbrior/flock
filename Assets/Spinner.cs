using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float rotateSpeed; // Speed of rotation
    public Transform target; // Center point to rotate around
    public GameObject prefab; // Prefab to spawn
    public float damage;
    public int instanceCount;
    public float radius = 0.4f; // Radius of the circle
    public float rotationOffest;

    private List<GameObject> spawnedInstances = new List<GameObject>();

    void Start()
    {
        SpawnRadialInstances();
    }

    void Update()
    {
        transform.position = target.position;
        transform.Rotate(0, 0, -rotateSpeed * Time.deltaTime);
    }

    public void IncreaseDamage(float pct) {
        damage = damage * (1f + pct);
        DeployDamage();
    }

    public void DeployDamage() {
        foreach (GameObject projObj in spawnedInstances) {
            Projectile projectile = projObj.GetComponent<Projectile>();
            projectile.damage = damage;
        }
    }

    public void SpawnRadialInstances()
    {
        ClearInstances();

        for (int i = 0; i < instanceCount; i++)
        {
            // Calculate the angle for this instance
            float angle = i * (360f / instanceCount);
            float angleRad = angle * Mathf.Deg2Rad;

            // Determine the position
            Vector3 position = new Vector3(
                target.position.x + Mathf.Cos(angleRad) * radius,
                target.position.y + Mathf.Sin(angleRad) * radius,
                target.position.z
            );

            // Instantiate the prefab at the calculated position
            GameObject instance = Instantiate(prefab, position, Quaternion.identity);

            // Rotate the instance to face outward
            instance.transform.rotation = Quaternion.Euler(0, 0, angle+rotationOffest);

            // Parent it to the spinner for rotation
            instance.transform.SetParent(transform);

            spawnedInstances.Add(instance);
        }
    }

    void ClearInstances()
    {
        foreach (GameObject instance in spawnedInstances)
        {
            if (instance != null)
                Destroy(instance);
        }
        spawnedInstances.Clear();
    }
}