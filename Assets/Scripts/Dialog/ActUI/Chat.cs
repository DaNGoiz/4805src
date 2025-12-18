using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnChatButtonClicked);

        Messenger.AddListener<int>(MsgType.DialogCharacterChanged, (characterId) =>
        {
            var character = CharacterManager.instance.GetCharacterByIndex(characterId);
            int affection = character.GetAffectionLevel();
            UIEffectUpdate(affection);
        });
    }

    private void OnChatButtonClicked()
    {
        Messenger.Broadcast(MsgType.ChatStart);
    }

    
    private void UIEffectUpdate(int affection)
    {
        if (affection >= 2)
        {
            button.interactable = true;
            GetComponent<Image>().color = Color.white;
        }
        else
        {
            button.interactable = false;
            GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
        }
    }
}