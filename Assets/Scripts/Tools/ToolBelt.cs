using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBelt : MonoBehaviour
{
    public Vector2 heading;
    public float shearRadius;
    public float wateringRadius;

    private Tool currentTool;
    private int totalToolCount;
    public bool allowedPlanting = true;
    [SerializeField] private GameObject pelletPrefab;
    [SerializeField] private Transform pelletSpawn;
    [SerializeField] private float pelletSpeed;
    [SerializeField] private bool useToolUI = false;
    [SerializeField] private ToolUI toolUI;

    void Start() {
        currentTool = Tool.Shears;
        totalToolCount = System.Enum.GetNames(typeof(Tool)).Length;
    }

    public void OnUseItem() {
        // Debug.Log("Tool used " + currentTool);
        if (currentTool == Tool.Slingshot) {
            UseSlingshot();
        } else if (currentTool == Tool.Shears) {
            UseShears();
        } else if (allowedPlanting && currentTool == Tool.SeedBag) {
            UseSeedBag();
        } else if (currentTool == Tool.WateringCan) {
            UseWateringCan();
        }
    }

    public void UseTool(Tool tool) {
        currentTool = tool;
        OnUseItem();
    }

    public void OnChangeTool() {
        int toolIdx = ((int) currentTool + 1) % totalToolCount;
        currentTool = (Tool) toolIdx;
        if (useToolUI) toolUI.SetActiveTool(toolIdx);
    }

    public void UseSlingshot() {
        Vector2 direction = (Vector2) (Pointer.Instance.transform.position - transform.position).normalized;
        GameObject pellet = Instantiate(pelletPrefab, (Vector2) transform.position + (direction * 0.1f), Quaternion.identity);
        Rigidbody2D pelletRb = pellet.GetComponent<Rigidbody2D>();
        pelletRb.velocity = direction * pelletSpeed;
        Destroy(pellet, 2f);
    }

    public void UseSlingshotAtTarget(Vector2 targetPosition) {
        Vector2 direction = Vector2.zero;
        Vector2 spawnLocation = Vector2.zero;
        if (pelletSpawn != null) {
            Vector2 pelletSpawnPosition = pelletSpawn.position;
            direction = (targetPosition - (Vector2) pelletSpawnPosition).normalized;
            spawnLocation = pelletSpawnPosition;
        } else {
            direction = (targetPosition - (Vector2) transform.position).normalized;
            spawnLocation = (Vector2) transform.position + (direction * 0.1f);
        }
        
        GameObject pellet = Instantiate(pelletPrefab, spawnLocation, Quaternion.identity);
        Rigidbody2D pelletRb = pellet.GetComponent<Rigidbody2D>();
        pelletRb.velocity = direction * pelletSpeed;
        Destroy(pellet, 2f);
    }

    private void UseShears() {
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, shearRadius);
        if (objectsInRange.Length > 0) {
            foreach (Collider2D obj in objectsInRange) {
                if (obj.TryGetComponent<ToolInteraction>(out ToolInteraction toolInteraction)) {
                    toolInteraction.UseTool(currentTool);
                }
            }
        }
    }

    private void UseSeedBag() {
        Vector2 spawnLocation = GetGridLocation(transform.position);

        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, 0.16f);
        if (objectsInRange.Length > 0) {
            foreach (Collider2D obj in objectsInRange) {
                if (obj.TryGetComponent<Crop>(out Crop crop)) {
                    if ((Vector2) crop.transform.position == spawnLocation) {
                        return;
                    }
                }
            }
        }
        CropManager.Instance.PlantCrop(spawnLocation);
    }

    private void UseWateringCan() {
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, wateringRadius);
        if (objectsInRange.Length > 0) {
            foreach (Collider2D obj in objectsInRange) {
                if (obj.TryGetComponent<ToolInteraction>(out ToolInteraction toolInteraction)) {
                    toolInteraction.UseTool(currentTool);
                }
            }
        }
    }

    private Vector2 GetGridLocation(Vector2 position)
    {
        float x = Mathf.Round((position.x - 0.08f) / 0.16f) * 0.16f + 0.08f;
        float y = Mathf.Round((position.y - 0.08f) / 0.16f) * 0.16f + 0.08f;

        return new Vector2(x, y);
    }
}
