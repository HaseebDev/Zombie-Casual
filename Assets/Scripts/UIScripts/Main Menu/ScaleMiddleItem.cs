using UnityEngine;
using UnityEngine.UI;
public class ScaleMiddleItem : MonoBehaviour
{
    public float middleItemScaleFactor = 1.2f; // The scale factor to apply to the middle item
    public float middleItemThreshold = 100f; // The distance threshold for the middle item
    public RectTransform ContentWindow;

    private ScrollRect scrollRect;
    private int elementIndexToSnap;
    public RectTransform[] elements;
    private float targetNormalizedPosition;
    // private UnityEngine.UI.Extensions.ScrollSnap scrollSnap;
    void SnaptoActiveLevel()
    {
        if (SaveGameHelper.GetMaxCampaignLevel() > 10)
        {
            elementIndexToSnap = SaveGameHelper.GetMaxCampaignLevel() % 10;
            if (elementIndexToSnap == 0)
            {
                elementIndexToSnap = 10;
            }
        }
        else
        {
            elementIndexToSnap = SaveGameHelper.GetMaxCampaignLevel();
        }
        float elementWidth = elements[0].rect.width;
        float totalWidth = elements.Length * elementWidth;
        float viewportWidth = scrollRect.viewport.rect.width;

        // Calculate the position to snap to based on the element index to snap
        float targetXPos = elementIndexToSnap * elementWidth + elementWidth / 2 - viewportWidth / 2;
        targetNormalizedPosition = targetXPos / (totalWidth - viewportWidth);

        // Snap to the element
        SnapTo(targetNormalizedPosition);
    }
    private void SnapTo(float normalizedPosition)
    {
        // Clamp the target position within [0, 1]
        targetNormalizedPosition = Mathf.Clamp01(normalizedPosition);

        // Snap to the target position
        scrollRect.normalizedPosition = new Vector2(targetNormalizedPosition, scrollRect.normalizedPosition.y);
    }

    private void GetallElements()
    {
        Debug.Log("Child Count : " + ContentWindow.childCount);
        for (int i = 0; i < ContentWindow.childCount; i++)
        {
            elements[i] = ContentWindow.GetChild(i).GetComponent<RectTransform>();
            // do something with the childGameObject reference
        }
    }

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        //scrollSnap = GetComponent< UnityEngine.UI.Extensions.ScrollSnap> ();
        scrollRect.onValueChanged.AddListener(OnScroll);

        Invoke("GetallElements", 1f);
        Invoke("SnaptoActiveLevel", 1.2f);


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
            //Debug.Log("Distance from center to item " + i + ": " + distance);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        //Debug.Log("Closest item index: " + closestIndex);

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
