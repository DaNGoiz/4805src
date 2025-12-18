using UnityEngine;
using UnityEngine.UI;

public class ChatFinish : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnFinishClicked);
    }

    private void OnFinishClicked()
    {
        Messenger.Broadcast(MsgType.ChatEnd);
    }
}
