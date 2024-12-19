using UnityEngine;

public class FriendlyUnit : MonoBehaviour
{
    void Awake() {
        if (TryGetComponent<Damagable>(out Damagable damagable)) {
            damagable.onDeath.AddListener(() => UnitManager.Instance.RemoveFriendlyUnit(transform));
        }
    }

    void Start() {
        UnitManager.Instance.AddFriendlyUnit(transform);
    }
}