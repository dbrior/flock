using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float rotateSpeed; // Speed of rotation
    public GameObject prefab; // Prefab to spawn
    public float damage;
    public int instanceCount;
    public float radius = 0.4f; // Base radius of the circle
    public float rotationOffest;
    public FloatRange radiusSize; // Assumed to have 'min' and 'max' values
    public bool hasDynamicRadius;
    public float radiusChangeSpeed;
    private List<GameObject> spawnedInstances = new List<GameObject>();
    private List<float> instanceAngles = new List<float>(); // Store the angles

    private float baseRadius;
    private float baseRotateSpeed;
    public float timeOffset;

    void Start()
    {
        baseRadius = radius; // Store the initial radius as the base radius
        baseRotateSpeed = rotateSpeed; // Store the initial rotate speed as the base rotate speed
        SpawnRadialInstances();
    }

    void Update()
    {
        // Update the radius if dynamic
        if (hasDynamicRadius)
        {
            // Smooth pulsing effect using sine wave
            float pulse = Mathf.Sin((Time.time+timeOffset) * radiusChangeSpeed) * 0.5f + 0.5f; // Value between 0 and 1
            radius = Mathf.Lerp(radiusSize.min, radiusSize.max, pulse);
        }

        // Adjust the rotateSpeed based on the current radius
        if (radius != 0)
        {
            rotateSpeed = baseRotateSpeed * (baseRadius / radius);
        }
        else
        {
            rotateSpeed = 0;
        }

        // Rotate the spinner
        transform.Rotate(0, 0, -rotateSpeed * Time.deltaTime);

        // Update the positions of the instances
        UpdateInstancePositions();
    }

    public void IncreaseDamage(float pct)
    {
        damage = damage * (1f + pct);
        DeployDamage();
    }

    public void SetDamage(float newDamage) {
        damage = newDamage;
        DeployDamage();
    }

    public void SetSpeed(float newSpeed) {
        baseRotateSpeed = newSpeed;
        rotateSpeed = newSpeed;
    }

    public void DeployDamage()
    {
        foreach (GameObject projObj in spawnedInstances)
        {
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
            instanceAngles.Add(angle); // Store the angle

            // Instantiate the prefab as a child of the spinner
            GameObject instance = Instantiate(prefab, transform);
            Projectile projectile = instance.GetComponent<Projectile>();
            projectile.damage = damage;

            if (transform.parent.gameObject.TryGetComponent<Collider2D>(out Collider2D collider)) {
                projectile.SetOwner(collider);
            }

            // Set the local position based on the angle and radius
            float angleRad = angle * Mathf.Deg2Rad;
            Vector3 localPosition = new Vector3(
                Mathf.Cos(angleRad) * radius,
                Mathf.Sin(angleRad) * radius,
                0
            );
            instance.transform.localPosition = localPosition;

            // Rotate the instance to face outward
            instance.transform.localRotation = Quaternion.Euler(0, 0, angle + rotationOffest);

            spawnedInstances.Add(instance);
        }
    }

    void UpdateInstancePositions()
    {
        for (int i = 0; i < spawnedInstances.Count; i++)
        {
            float angle = instanceAngles[i];
            float angleRad = angle * Mathf.Deg2Rad;

            // Update the local position based on the current radius
            Vector3 localPosition = new Vector3(
                Mathf.Cos(angleRad) * radius,
                Mathf.Sin(angleRad) * radius,
                0
            );
            spawnedInstances[i].transform.localPosition = localPosition;
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
        instanceAngles.Clear();
    }
}