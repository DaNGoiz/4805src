using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;
    private List<Character> characters = new List<Character>();

    void Awake()
    {
        instance = this;
        InstantiateAllCharacters();
    }

    // 生成并收集所有角色实例（读表生成角色）
    private void InstantiateAllCharacters()
    {
        int npcCount = PhysicalWorldManager.NPCCount;
        for (int i = 0; i < npcCount; i++)
        {
            GameObject npcObj = new GameObject("NPC_" + i);
            Character character = npcObj.AddComponent<Character>();

            character.SetId(i);
            character.Initialize();

            characters.Add(character);
        }

    }

    public Character GetCharacterByIndex(int index)
    {
        return characters[index];
    }

    private float CheckTopicSpreadSingleRoom(string topic, string room)
    {
        // 读表，检查当前这个房间所有的人，计算某个话题的认同概率（知道=认同）
        

        return 0.5f;
    }

    private void SetCharacterTopic(int characterId, string intel)
    {
        // 设置某个角色正在传播的话题
        characters.Find(c => c.GetId() == characterId).SetSpreadingIntel(intel);
    }

    public List<Character> GetCharacterList()
    {
        return characters;
    }
}