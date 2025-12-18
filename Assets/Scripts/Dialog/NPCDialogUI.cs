using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCDialogUI : MonoBehaviour
{
    // ====== 角色基础数据（从配置 + CharacterManager 读） ======
    public int characterId;          // 这个可以存“角色索引”
    private string characterName;
    private int power;
    private int influence;
    private string interest;
    private string groupTag;
    private Sprite characterSprite;

    private int affection;            // 好感度，实时更新
    private string spreadingTopic;    // 当前传播话题

    // ====== UI 引用（结构是固定的，就直接缓存） ======
    [SerializeField] private GameObject background;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI characterStatsText;
    [SerializeField] private TextMeshProUGUI characterSpreadText;
    [SerializeField] private TextMeshProUGUI affectionText;
    [SerializeField] private GameObject InteractionPanel;
    [SerializeField] private GameObject ChatPanel;

    private DialogUIPanelDoubleIdle dialogUI;

    // ====== 配置表 ======
    private cfg.story.npc_basic_info npcData;
    private cfg.story.Tbnpc_basic_info npcMap;

    // ---------- 生命周期 ----------

    private void Awake()
    {
        if (background == null)
            background = transform.Find("Background")?.gameObject;

        if (characterStatsText == null)
            characterStatsText = transform.Find("Background/Info/Basic/Detail")
                .GetComponent<TextMeshProUGUI>();
        if (characterSpreadText == null)
            characterSpreadText = transform.Find("Background/Info/Basic/Spreading")
                .GetComponent<TextMeshProUGUI>();
        if (affectionText == null)
            affectionText = transform.Find("Background/Info/Affection/Detail")
                .GetComponent<TextMeshProUGUI>();

        if (nameText == null)
            nameText = transform.Find("Background/Info/Basic/Name")
                ?.GetComponent<TextMeshProUGUI>();
        if (characterImage == null)
            characterImage = transform.Find("Background/Portrait")
                ?.GetComponent<Image>();
        
        if (InteractionPanel == null)
            InteractionPanel = transform.Find("Background/Interaction")?.gameObject;

        if (ChatPanel == null)
            ChatPanel = transform.Find("Background/IntelligenceDrop")?.gameObject;

        dialogUI = FindObjectOfType<DialogUIPanelDoubleIdle>();

        npcMap = new cfg.story.Tbnpc_basic_info(
            SimpleJSON.JSON.Parse(
                Resources.Load<TextAsset>("Datas/Config/story_tbnpc_basic_info").text
            )
        );

        if (background != null)
            background.SetActive(false);
    }

    private void OnEnable()
    {
        Messenger.AddListener<int>(MsgType.OnWorldCharacterClicked, PlayDialog);
        Messenger.AddListener(MsgType.CloseDialog, HideDialog);
        Messenger.AddListener(MsgType.ChatStart, ShowChatPanel);
        Messenger.AddListener(MsgType.ChatEnd, ShowInteractionPanel);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<int>(MsgType.OnWorldCharacterClicked, PlayDialog);
        Messenger.RemoveListener(MsgType.CloseDialog, HideDialog);
    }

    // ---------- 数据加载 ----------
    private void LoadNPCData(int characterIndex)
    {
        characterId = characterIndex;
        Messenger.Broadcast<int>(MsgType.DialogCharacterChanged, characterIndex);

        // 1. 从角色管理器拿到这个角色的运行时对象
        Character character = CharacterManager.instance.GetCharacterByIndex(characterIndex);
        if (character == null)
        {
            Debug.LogError($"[NPCDialogUI] Character index {characterIndex} not found in CharacterManager.");
            return;
        }

        int configId = character.GetId();

        npcData = npcMap.Get(configId);
        if (npcData == null)
        {
            Debug.LogError($"[NPCDialogUI] npcMap.Get({configId}) returned null.");
            return;
        }

        // 2. 从配置表拿静态信息
        characterName = npcData.Name;
        power = npcData.Power;
        influence = npcData.Influence;
        interest = npcData.Interest;
        groupTag = npcData.Grouptag;

        // 3. 从角色对象拿动态信息
        affection = character.GetAffectionLevel();
        spreadingTopic = character.GetSpreadingTopic();

        // 4. 填 UI
        if (characterStatsText != null)
            characterStatsText.text = $"Power: {power}\nInfluence: {influence}\nInterest: {interest}";

        if (characterSpreadText != null)
            characterSpreadText.text = spreadingTopic;

        if (affectionText != null)
            affectionText.text = affection.ToString();

        // 头像
        characterSprite = Resources.Load<Sprite>(
            "Textures/Character/Illustration/" + characterName
        );
        if (characterImage != null)
            characterImage.sprite = characterSprite;

        if (nameText != null)
            nameText.text = characterName;

        Debug.Log($"[NPCDialogUI] LoadNPCData: index={characterIndex}, configId={configId}, name={characterName}");
    }

    // ---------- UI 显示 / 隐藏 ----------

    private void HideDialog()
    {
        if (background != null)
            background.SetActive(false);
    }

    private void PlayDialog(int index)
    {
        LoadNPCData(index);

        if (background != null)
            background.SetActive(true);

        if (dialogUI != null && npcData != null)
        {
            dialogUI.SetDialog(
                characterSprite,
                npcData.Name,
                "你好，我是" + npcData.Name + "，这是测试对话框" // TODO：以后换成插件返回的文本
            );
        }
    }

    private void ShowChatPanel()
    {
        if (InteractionPanel != null)
            InteractionPanel.SetActive(false);
        if (ChatPanel != null)
            ChatPanel.SetActive(true);
    }
    
    private void ShowInteractionPanel()
    {
        if (InteractionPanel != null)
            InteractionPanel.SetActive(true);
        if (ChatPanel != null)
            ChatPanel.SetActive(false);
    }
}
