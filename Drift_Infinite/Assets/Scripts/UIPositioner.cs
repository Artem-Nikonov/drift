using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPositioner : MonoBehaviour
{
    public RectTransform targetElement;
    public float YOffset;

    void Update()
    {
        if (targetElement != null)
        {
            Vector2 targetPosition = new(transform.position.x, targetElement.position.y + YOffset);
            transform.position = targetPosition;
        }
    }
}
