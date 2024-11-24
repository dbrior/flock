using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    public float scaleMultiplier = 1.1f; // How much to grow the UI element.
    public float animationDuration = 0.1f; // How fast the scaling happens.

    private Coroutine scalingCoroutine;

    private void Start()
    {
        originalScale = transform.localScale;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (scalingCoroutine != null) StopCoroutine(scalingCoroutine);
        scalingCoroutine = StartCoroutine(ScaleTo(originalScale * scaleMultiplier));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (scalingCoroutine != null) StopCoroutine(scalingCoroutine);
        scalingCoroutine = StartCoroutine(ScaleTo(originalScale));
    }

    private IEnumerator ScaleTo(Vector3 targetScale)
    {
        float time = 0;
        Vector3 startScale = transform.localScale;

        while (time < animationDuration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, time / animationDuration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }

}