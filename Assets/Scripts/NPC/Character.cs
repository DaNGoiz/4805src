using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Character : MonoBehaviour
{
    private int id;
    private int affectionLevel; // 好感度
    private int affectionDetail; // 好感度细节，会被折算成好感度
    private string spreadingIntelligence; // 正在传播的话题
    private List<string> knownIntelligence = new List<string>(); // 已解锁的话题

    private cfg.story.Tbnpc_basic_info npcMap;
    private cfg.story.npc_basic_info npcInfo;

    private int power;
    private int influence;
    private string interest;
    private string grouptag;

    private string spreadingTopic = "None";


    private bool isGreetingDone; // 是否完成了初次问候

    // 模拟才会用到，现在用不到
    private Character interactionTarget; // 互动对象
    private Dictionary<Character, int> affectionToNPC = new Dictionary<Character, int>(); // 对其他NPC的好感度


    public void Initialize()
    {
        npcMap = new cfg.story.Tbnpc_basic_info(
            SimpleJSON.JSON.Parse(Resources.Load<TextAsset>("Datas/Config/story_tbnpc_basic_info").text)
        );
        npcInfo = npcMap.Get(id);
        affectionLevel = 3;
        affectionDetail = 0;
        power = npcInfo.Power;
        influence = npcInfo.Influence;
        interest = npcInfo.Interest;
        grouptag = npcInfo.Grouptag;
        spreadingTopic = "";
        // SetKnowTopic("真王"); // debug
    }

    public void AffectionChange(int delta)
    {
        if (affectionLevel > 5) return; // 最高好感度5
        affectionDetail += delta;
        // 折算算法，暂时是每10点好感度细节提升1点好感度
        if (affectionDetail >= 3)
        {
            affectionLevel += 1;
            affectionDetail = 0;
        }
    }

    // TODO: 如果更新了日期，需要重置问候状态
    public void GreetingDone()
    {
        isGreetingDone = true;
    }

    public void SetId(int characterId)
    {
        id = characterId;
    }

    public int GetId()
    {
        return id;
    }

    public void SetSpreadingIntel(string intel)
    {
        spreadingIntelligence = intel;
    }

    public string GetSpreadingIntel()
    {
        return spreadingIntelligence;
    }

    public List<string> GetKnownIntel()
    {
        return knownIntelligence;
    }

    public void SetInteractionTarget(Character target)
    {
        interactionTarget = target;
    }

    public int GetAffectionLevel()
    {
        return affectionLevel;
    }

    public string GetSpreadingTopic()
    {
        return spreadingTopic;
    }

    public int GetPower()
    {
        return power;
    }

    public int GetInfluence()
    {
        return influence;
    }

    public string GetInterest()
    {
        return interest;
    }

    public string GetGroupTag()
    {
        return grouptag;
    }

    public bool KnowTopic(string topic)
    {
        foreach (string knownTopic in knownIntelligence)
        {
            if (knownTopic == topic)
            {
                return true;
            }
        }
        return false;
    }

    public bool SetKnowTopic(string topic)
    {
        if (!KnowTopic(topic))
        {
            knownIntelligence.Add(topic);
            return true;
        }
        return false;
    }

    public bool IsInterested(string tag)
    {
        return interest == tag;
    }
}