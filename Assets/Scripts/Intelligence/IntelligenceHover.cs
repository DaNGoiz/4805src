using UnityEngine;
using UnityEngine.EventSystems;

public class IntelligenceHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 获取自身的情报ID，然后广播显示面板的消息
        IntelligenceUI intelligenceUI = GetComponent<IntelligenceUI>();
        string intelligenceName = intelligenceUI.GetIntelligenceName();
        Messenger.Broadcast(MsgType.ShowSpreadPanel, intelligenceName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Messenger.Broadcast(MsgType.HideSpreadPanel);
    }
}
