using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicScrollView : MonoBehaviour
{
    public RectTransform contentRectTransform;
    public float elementHeight;

    private int numberOfElements = 0;

    public void AddElement()
    {
        numberOfElements++;
        UpdateContentSize();
    }

    public void RemoveElement()
    {
        numberOfElements--;
        UpdateContentSize();
    }

    private void UpdateContentSize()
    {
        // Calculate the total height needed for all elements
        float totalHeight = elementHeight * numberOfElements;

        // Get the current size of the content
        Vector2 size = contentRectTransform.sizeDelta;

        // Update the height of the content
        size.y = totalHeight;

        // Apply the new size to the content
        contentRectTransform.sizeDelta = size;
    }
}
