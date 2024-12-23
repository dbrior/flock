using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance {get; private set;}

    private List<Transform> friendlyUnits = new List<Transform>();
    private List<Transform> enemyUnits = new List<Transform>();

    private Collider2D safezoneCol;

    void Awake() {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        safezoneCol = GetComponent<Collider2D>();
    }

    public void AddFriendlyUnit(Transform newTransform) {
        if (newTransform.GetComponent<Collider2D>().bounds.Intersects(safezoneCol.bounds)) return;
        friendlyUnits.Add(newTransform);
    }
    public void RemoveFriendlyUnit(Transform transformToRemove) {
        friendlyUnits.Remove(transformToRemove);
    }

    public void AddEnemyUnit(Transform newTransform) {
        enemyUnits.Add(newTransform);
    }
    public void RemoveEnemyUnit(Transform transformToRemove) {
        enemyUnits.Remove(transformToRemove);
    }

    public Transform GetClosestFriendlyUnit(Transform inputTransform) {
        Vector2 inputPos = inputTransform.position;
        Transform closest = null;
        float minSqrDist = float.MaxValue;
        
        for (int i = friendlyUnits.Count-1; i >= 0; i--) {
            if (friendlyUnits[i] == null) {
                friendlyUnits.RemoveAt(i);
                continue;
            }
            float sqrDist = ((Vector2)friendlyUnits[i].position - inputPos).sqrMagnitude;
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                closest = friendlyUnits[i];
            }
        }
        
        return closest;
    }

    public Transform GetClosestEnemyUnit(Transform inputTransform) {
        Vector2 inputPos = inputTransform.position;
        Transform closest = null;
        float minSqrDist = float.MaxValue;
        
        for (int i = enemyUnits.Count-1; i >= 0; i--) {
            if (enemyUnits[i] == null) {
                enemyUnits.RemoveAt(i);
                continue;
            }
            float sqrDist = ((Vector2)enemyUnits[i].position - inputPos).sqrMagnitude;
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                closest = enemyUnits[i];
            }
        }
        
        return closest;
    }

    public void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.TryGetComponent<FriendlyUnit>(out FriendlyUnit unit)) {
            RemoveFriendlyUnit(unit.transform);
        }
    }

    public void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.TryGetComponent<FriendlyUnit>(out FriendlyUnit unit)) {
            AddFriendlyUnit(unit.transform);
        }
    }
}
