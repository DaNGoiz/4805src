using UnityEngine;
using UnityEngine.UI;

public class DragIcon : MonoBehaviour
{
    private RectTransform rectTransform;

    public static DragIcon Create(Sprite sprite)
    {
        var canvas = GameObject.FindObjectOfType<Canvas>();
        GameObject iconGO = new GameObject("DragIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        iconGO.transform.SetParent(canvas.transform, false);

        var image = iconGO.GetComponent<Image>();
        image.sprite = sprite;
        image.raycastTarget = false;

        var instance = iconGO.AddComponent<DragIcon>();
        return instance;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            Input.mousePosition,
            null,
            out pos
        );
        rectTransform.anchoredPosition = pos;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
