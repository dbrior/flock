using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    void Awake() {
        if (TryGetComponent<Damagable>(out Damagable damagable)) {
            damagable.onDeath.AddListener(() => UnitManager.Instance.RemoveEnemyUnit(transform));
        }
    }

    void Start() {
        UnitManager.Instance.AddEnemyUnit(transform);
    }
}