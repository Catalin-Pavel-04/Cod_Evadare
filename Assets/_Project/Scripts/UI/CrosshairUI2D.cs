using UnityEngine;

public class CrosshairUI2D : MonoBehaviour
{
    [SerializeField] private RectTransform crosshair;
    [SerializeField] private bool hideSystemCursor = true;

    private bool previousCursorVisible = true;

    private void OnEnable()
    {
        previousCursorVisible = Cursor.visible;

        if (hideSystemCursor)
        {
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (crosshair != null)
        {
            crosshair.position = Input.mousePosition;
        }
    }

    private void OnDisable()
    {
        Cursor.visible = previousCursorVisible;
    }

    private void OnDestroy()
    {
        Cursor.visible = previousCursorVisible;
    }
}
