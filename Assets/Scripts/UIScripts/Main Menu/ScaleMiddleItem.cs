using UnityEngine;
using UnityEngine.UI;

public class ScaleMiddleItem : MonoBehaviour
{
    public float middleItemScaleFactor = 1.2f; // The scale factor to apply to the middle item
    public float middleItemThreshold = 100f; // The distance threshold for the middle item

    private ScrollRect scrollRect;
   // private UnityEngine.UI.Extensions.ScrollSnap scrollSnap;
    void SnaptoActiveLevel()
    {
        //RectTransform contentRect = scrollRect.content.GetComponent<RectTransform>();
        //float viewportWidth = scrollRect.viewport.rect.width;
        //float contentWidth = contentRect.rect.width;
        //float itemWidth = contentWidth / contentRect.childCount;
        //float middlePosition = viewportWidth / 2f;
        //float leftmostPosition = -contentWidth / 2f + itemWidth / 2f;

        //float closestDistance = Mathf.Infinity;
        //RectTransform closestElement = null;

        //// Loop through all child elements of the content object and find the one closest to the middle of the viewport
        //foreach (RectTransform child in contentRect)
        //{
        //    float distance = Mathf.Abs(child.position.x - middlePosition);

        //    if (distance < closestDistance)
        //    {
        //        closestDistance = distance;
        //        closestElement = child;
        //    }
        //}

        //if (closestElement != null)
        //{
        //    // Offset the xPosition to ensure it's always positive
        //    float xPosition = closestElement.position.x + Mathf.Abs(leftmostPosition);

        //    float normalizedPosition = xPosition / (contentWidth - viewportWidth);
        //    normalizedPosition = Mathf.Clamp01(normalizedPosition);

        //    scrollRect.normalizedPosition = new Vector2(normalizedPosition, 0f);
        //    scrollRect.content.anchoredPosition = new Vector2(xPosition - closestElement.rect.width / 2f, 0f);
        //}
        //Debug.Log("itemToSnapName: " + itemRectTransform.name);
        //Debug.Log("XPosition: " + xPosition);
        //Debug.Log("Normalized Pos " + normalizedPosition);
        //Debug.Log("Viewport width: " + viewportWidth);
        //Debug.Log("Content width: " + contentWidth);
        //Debug.Log("Indext to Snap to" + indexToSnapTo);
        // Snap the scroll view to the content item
        //scrollRect.normalizedPosition = new Vector2(normalizedPosition, 0f);
    }

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        //scrollSnap = GetComponent< UnityEngine.UI.Extensions.ScrollSnap> ();
        scrollRect.onValueChanged.AddListener(OnScroll);

        Invoke("SnaptoActiveLevel", 2f);


        //float closestDistance = float.MaxValue;
        //int closestIndex = 0;

        //for (int i = 0; i < scrollRect.content.childCount; i++)
        //{
        //    RectTransform childTransform1 = scrollRect.content.GetChild(i).GetComponent<RectTransform>();
        //    Vector3 center = childTransform1.position + new Vector3(childTransform1.rect.width * childTransform1.pivot.x, -childTransform1.rect.height * childTransform1.pivot.y, 0f);
        //    float distance = Vector2.Distance(scrollRect.viewport.rect.center, center);

        //    if (distance < closestDistance)
        //    {
        //        closestDistance = distance;
        //        closestIndex = i;
        //    }
        //}

        //// Scale the closest content item to the middle of the scroll view
        //for (int i = 0; i < scrollRect.content.childCount; i++)
        //{
        //    RectTransform childTransform2 = scrollRect.content.GetChild(i).GetComponent<RectTransform>();
        //    if (i == closestIndex)
        //    {
        //        childTransform2.localScale = Vector3.one * middleItemScaleFactor;
        //    }
        //    else
        //    {
        //        childTransform2.localScale = Vector3.one;
        //    }
        //}



    }

    private void OnScroll(Vector2 position)
    {
        float closestDistance = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            RectTransform childTransform = scrollRect.content.GetChild(i).GetComponent<RectTransform>();
            Vector3 center = childTransform.position + new Vector3(childTransform.rect.width * childTransform.pivot.x, -childTransform.rect.height * childTransform.pivot.y, 0f);
            float distance = Vector2.Distance(scrollRect.viewport.rect.center, center);
            Debug.Log("Distance from center to item " + i + ": " + distance);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        Debug.Log("Closest item index: " + closestIndex);

        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            RectTransform childTransform = scrollRect.content.GetChild(i).GetComponent<RectTransform>();
            Vector3 center = childTransform.position + new Vector3(childTransform.rect.width * childTransform.pivot.x, -childTransform.rect.height * childTransform.pivot.y, 0f);
            float distance = Vector2.Distance(scrollRect.viewport.rect.center, center);
            if (i == closestIndex)
            {
                childTransform.localScale = Vector3.one * middleItemScaleFactor;
            }
            else
            {
                childTransform.localScale = Vector3.one;
            }
        }
    }
}
