using UnityEngine;

public class ProgressPanel : MonoBehaviour
{
    private GameObject progressPanel;
    private void Awake()
    {
        progressPanel = transform.Find("Background")?.gameObject;
        Messenger.AddListener<string>(MsgType.ShowSpreadPanel, ShowPanel);
        Messenger.AddListener(MsgType.HideSpreadPanel, HidePanel);

        Messenger.AddListener(MsgType.CloseMap, HidePanel);
    }
    private void RefreshUI(string intel)
    {
        // 刷新所有面板
        Messenger.Broadcast<string>(MsgType.ShowSpreadDetail, intel);

    }

    private void ShowPanel(string intel)
    {
        // 显示面板（背景逐渐显示）
        progressPanel.SetActive(true);
        RefreshUI(intel);
    }

    private void HidePanel()
    {
        // 隐藏面板（背景逐渐消失）
        progressPanel.SetActive(false);
    }

}