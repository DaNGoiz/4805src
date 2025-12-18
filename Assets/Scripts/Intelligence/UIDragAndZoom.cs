using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragAndZoom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("拖拽边界")]
    public Vector2 minPosition = new Vector2(-500, -500);
    public Vector2 maxPosition = new Vector2(500, 500);

    [Header("缩放限制")]
    public float minScale = 0.5f;
    public float maxScale = 2f;
    public float zoomSpeed = 0.1f;

    private RectTransform rectTransform;
    private bool isPointerOver = false;
    private bool isDragging = false;
    private Vector2 lastMousePos;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!isPointerOver) return;

        // 拖拽
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;

            Vector2 newPos = rectTransform.anchoredPosition + delta;
            newPos.x = Mathf.Clamp(newPos.x, minPosition.x, maxPosition.x);
            newPos.y = Mathf.Clamp(newPos.y, minPosition.y, maxPosition.y);
            rectTransform.anchoredPosition = newPos;
        }

        // 缩放
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float scale = rectTransform.localScale.x + scroll * zoomSpeed;
            scale = Mathf.Clamp(scale, minScale, maxScale);
            rectTransform.localScale = new Vector3(scale, scale, 1f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
    }
}
