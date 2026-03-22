using UnityEngine;
using UnityEngine.InputSystem;

public class CursorSiembra : MonoBehaviour
{
    RectTransform rectTransform;
    Canvas canvas;

    void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>(true);
    }

    void Update()
    {
        if (rectTransform == null || canvas == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            rectTransform.position = mousePos;
        }
        else
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                mousePos,
                canvas.worldCamera,
                out Vector2 localPoint))
            {
                rectTransform.localPosition = localPoint;
            }
        }
    }
}