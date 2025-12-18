using UnityEngine;
using UnityEngine.UI;

public class Greeting : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnGreetButtonClicked);
    }

    private void OnGreetButtonClicked()
    {
        // 读取角色的greeting文本
        DialogUIPanelDoubleIdle dialogUI = FindObjectOfType<DialogUIPanelDoubleIdle>();
        dialogUI.SetDialog(null, "Fortune Teller", "Hello! Nice to meet you.", "left");

        NPCDialogUI npcDialogUI = FindObjectOfType<NPCDialogUI>();
        int characterId = npcDialogUI.characterId;
        var character = CharacterManager.instance.GetCharacterByIndex(characterId);
        character.AffectionChange(3);
        character.GreetingDone();

    }
}