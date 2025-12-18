using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotalProgressUI : MonoBehaviour
{
    private TMPro.TextMeshProUGUI progressText;
    private Slider progressBar;
    private float completionPercentage;
    private cfg.story.Tbnpc_basic_info npcMap;

    void Start()
    {
        progressText = transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();
        progressBar = transform.Find("Slider").GetComponent<Slider>();

        npcMap = new cfg.story.Tbnpc_basic_info(
            SimpleJSON.JSON.Parse(Resources.Load<TextAsset>("Datas/Config/story_tbnpc_basic_info").text)
        );

        Messenger.AddListener<string>(MsgType.ShowSpreadDetail, UpdateUI);
    }

      private void CalculateCompletionPercentage(string intel)
    {
        int totalCharacters = 0;
        int agreeingCharacters = 0;

        // 获取当前时间下房间里的所有人，读取他们是否同意某个情报，然后计算概率
        int npcCount = PhysicalWorldManager.NPCCount;
        List<Character> characters = new List<Character>();
        for (int i = 0; i < npcCount; i++)
        {
            Character character = CharacterManager.instance.GetCharacterByIndex(i);
            characters.Add(character);
            totalCharacters++;
        }

        foreach (Character character in characters)
        {
            if (character.KnowTopic(intel))
            {
                agreeingCharacters++;
            }
        }

        if (totalCharacters > 0)
        {
            completionPercentage = (float)agreeingCharacters / totalCharacters * 100.0f;
        }
    }

    private void UpdateUI(string intel)
    {
        CalculateCompletionPercentage(intel);
        progressBar.value = completionPercentage;
        progressText.text = Mathf.FloorToInt(completionPercentage).ToString() + "%";
    }

    private string GetNPCPlaceByTime(int time, int index)
    {
        cfg.story.npc_basic_info npcInfo = npcMap.Get(index);
        if (time == 0)
            return npcInfo.Hour0;
        else if (time == 3)
            return npcInfo.Hour3;
        else if (time == 6)
            return npcInfo.Hour6;
        else if (time == 9)
            return npcInfo.Hour9;
        else if (time == 12)
            return npcInfo.Hour12;
        else if (time == 15)
            return npcInfo.Hour15;
        else if (time == 18)
            return npcInfo.Hour18;
        else if (time == 21)
            return npcInfo.Hour21;
        else
            return "";
    }
}