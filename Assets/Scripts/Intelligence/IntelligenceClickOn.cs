using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class IntelligenceClickOn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Sprite dragSprite;
    private DragIcon currentDragIcon;

    private void Start()
    {
        dragSprite = transform.Find("IntImage").GetComponent<Image>().sprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (dragSprite == null) return;
        currentDragIcon = DragIcon.Create(dragSprite);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (currentDragIcon == null) return;

        // 判断当前鼠标是否在任何DropZone上
        var pointerRaycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, pointerRaycastResults);

        foreach (var result in pointerRaycastResults)
        {
            var zone = result.gameObject.GetComponent<DropZone>();
            if (zone != null)
            {
                zone.ReceiveDroppedSprite(dragSprite);
                break;
            }
        }

        currentDragIcon.DestroySelf();
        currentDragIcon = null;
    }
}
