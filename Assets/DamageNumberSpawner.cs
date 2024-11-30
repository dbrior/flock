using System.Collections;
using UnityEngine;
using TMPro;

public class DamageNumberSpawner : MonoBehaviour
{
    public static DamageNumberSpawner Instance { get; private set; }

    public Canvas worldSpaceCanvas; // Assign the canvas in the inspector
    public GameObject damageNumberPrefab; // Assign the prefab for the damage number

    [SerializeField] private Color critColor;

    void Awake() {
        if (Instance == null) {Instance = this;}
        else {Destroy(gameObject);}
    }

    public void SpawnDamageNumber(Vector3 position, string damageText, bool isCrit)
    {
        // Instantiate the damage number prefab under the world-space canvas
        GameObject damageNumber = Instantiate(damageNumberPrefab, worldSpaceCanvas.transform);

        // Spawn offset
        float offset = Random.Range(-0.075f, 0.075f);
        Vector3 spawnOffset = new Vector3(offset, 0, 0);

        // Set the position in local space
        RectTransform rectTransform = damageNumber.GetComponent<RectTransform>();
        rectTransform.position = position + spawnOffset; // Position in world space works for 2D

        // Set the damage text
        TextMeshProUGUI damageTextObj = damageNumber.GetComponent<TextMeshProUGUI>();
        damageTextObj.text = damageText;

        // Set Crit color
        if (isCrit) {
            damageTextObj.color = critColor;
            damageTextObj.fontSize *= 1.5f;
        }

        // Optional: Start fade or animation
        StartCoroutine(AnimateAndDisable(damageNumber));
    }

    private IEnumerator AnimateAndDisable(GameObject damageNumber)
    {
        // Example animation: Move up and fade out
        float duration = 1f;
        Vector3 startPosition = damageNumber.transform.position;
        Vector3 endPosition = startPosition + Vector3.up * 0.1f;
        TextMeshProUGUI text = damageNumber.GetComponent<TextMeshProUGUI>();
        Color startColor = text.color;

        float elapsed = 0;
        while (elapsed < duration)
        {
            // Move the number up
            damageNumber.transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / duration);

            // Fade out
            text.color = new Color(startColor.r, startColor.g, startColor.b, 1 - (elapsed / duration));

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Destroy or pool the object
        Destroy(damageNumber);
    }
}
