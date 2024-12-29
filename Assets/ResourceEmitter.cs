using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class ResourceEmitter : MonoBehaviour
{
    [SerializeField] private float emitIntervalSec;
    [SerializeField] private UnityEvent onEmit;

    void Start() {
        StartCoroutine("Timer");
    }

    IEnumerator Timer() {
        while (true) {
            yield return new WaitForSeconds(emitIntervalSec);
            onEmit?.Invoke();
        }
    }
}
