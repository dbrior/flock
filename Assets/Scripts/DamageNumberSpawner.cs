using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public enum DamageNumberType {
    Normal,
    Crit,
    Heal
}

[System.Serializable]
public enum StatusIconType {
    Block,
    Death
}

[System.Serializable]
public class StatusSprite {
    public StatusIconType type;
    public Sprite sprite;
}

public class DamageNumberSpawner : MonoBehaviour
{
    public static DamageNumberSpawner Instance { get; private set; }

    public Canvas worldSpaceCanvas; // Assign the canvas in the inspector
    public GameObject damageNumberPrefab; // Assign the prefab for the damage number
    public GameObject statusIconPrefab;
    public List<StatusSprite> statusSpritesList;
    private Dictionary<StatusIconType,Sprite> statusSprites = new Dictionary<StatusIconType,Sprite>();

    [SerializeField] private Color critColor;
    [SerializeField] private Color healColor;

    void Awake() {
        if (Instance == null) {Instance = this;}
        else {Destroy(gameObject);}
    }

    void Start() {
        foreach (StatusSprite statusSprite in statusSpritesList) {
            statusSprites[statusSprite.type] = statusSprite.sprite;
        }
    }

    public void SpawnDamageNumber(Vector3 position, string damageText, DamageNumberType type)
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
        if (type == DamageNumberType.Crit) {
            damageTextObj.color = critColor;
            damageTextObj.fontSize *= 1.5f;
        } else if (type == DamageNumberType.Heal) {
            damageTextObj.color = healColor;
        }

        // Optional: Start fade or animation
        StartCoroutine(AnimateAndDisable(damageNumber));
    }

    public void SpawnStatusIcon(Vector3 position, StatusIconType type)
    {
        GameObject statusIcon = Instantiate(statusIconPrefab, worldSpaceCanvas.transform);
        Image image = statusIcon.GetComponent<Image>();
        image.sprite = statusSprites[type];

        // Spawn offset
        float offset = Random.Range(-0.075f, 0.075f);
        Vector3 spawnOffset = new Vector3(offset, 0, 0);

        // Set the position in local space
        RectTransform rectTransform = statusIcon.GetComponent<RectTransform>();
        rectTransform.position = position + spawnOffset; // Position in world space works for 2D

        // Optional: Start fade or animation
        StartCoroutine(AnimateAndDisable(statusIcon));
    }

    private IEnumerator AnimateAndDisable(GameObject damageNumber)
    {
        // Example animation: Move up and fade out
        float duration = 1f;
        Vector3 startPosition = damageNumber.transform.position;
        Vector3 endPosition = startPosition + Vector3.up * 0.1f;

        if (damageNumber.TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI text)) {
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
        } else if (damageNumber.TryGetComponent<Image>(out Image image)) {
            Color startColor = image.color;

            float elapsed = 0;
            while (elapsed < duration)
            {
                // Move the number up
                damageNumber.transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / duration);

                // Fade out
                image.color = new Color(startColor.r, startColor.g, startColor.b, 1 - (elapsed / duration));

                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        

        // Destroy or pool the object
        Destroy(damageNumber);
    }
}
