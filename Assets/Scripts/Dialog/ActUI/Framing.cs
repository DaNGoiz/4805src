using UnityEngine;
using UnityEngine.UI;

public class Framing : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnFrameButtonClicked);

        Messenger.AddListener<int>(MsgType.DialogCharacterChanged, (characterId) =>
        {
            var character = CharacterManager.instance.GetCharacterByIndex(characterId);
            int affection = character.GetAffectionLevel();
            UIEffectUpdate(affection);
        });
    }

    private void OnFrameButtonClicked()
    {
        
    }

    // 备注：需要在每次加载的时候调用，广播注入affection
    private void UIEffectUpdate(int affection) // debug
    {
        if (affection >= 3)
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