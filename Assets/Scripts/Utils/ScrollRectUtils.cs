using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static class ScrollRectUtils
{
    public static void SnapTo(this ScrollRect scroller, RectTransform child)
    {
        Canvas.ForceUpdateCanvases();

        var contentPos = (Vector2) scroller.transform.InverseTransformPoint(scroller.content.position);
        var childPos = (Vector2) scroller.transform.InverseTransformPoint(child.position);
        var endPos = contentPos - childPos;
        // If no horizontal scroll, then don't change contentPos.x
        if (!scroller.horizontal) endPos.x = contentPos.x;
        // If no vertical scroll, then don't change contentPos.y
        if (!scroller.vertical) endPos.y = contentPos.y;
        scroller.content.anchoredPosition = endPos;
    }

    /// <summary>
    /// Thanks to https://stackoverflow.com/a/50191835
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="child"></param>
    /// <returns></returns>
    public static IEnumerator BringChildIntoView(this UnityEngine.UI.ScrollRect instance, RectTransform child, float distanceUpdate = 0)
    {
        // Canvas.ForceUpdateCanvases();
        // yield return new WaitForEndOfFrame();

        Vector2 viewportLocalPosition = instance.viewport.localPosition;
        Vector2 childLocalPosition = child.localPosition;
        Vector2 result = new Vector2(
            0 - (viewportLocalPosition.x + childLocalPosition.x),
            0 - (viewportLocalPosition.y + childLocalPosition.y)
        );

        // Debug.LogError("OLD " + instance.content.localPosition + " CHILD " + childLocalPosition);
        // Debug.LogError(result);
        // Debug.LogError(Vector3.Distance(result, instance.content.localPosition));

        if (Vector3.Distance(result, instance.content.localPosition) > distanceUpdate)
        {
            // instance.content.localPosition = result;

            instance.content.DOLocalMove(result, 0.25f);
            
            yield return new WaitForEndOfFrame();

            instance.horizontalNormalizedPosition = Mathf.Clamp(instance.horizontalNormalizedPosition, 0f, 1f);
            instance.verticalNormalizedPosition = Mathf.Clamp(instance.verticalNormalizedPosition, 0f, 1f);
            
        }
    }
}