using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InformationModelSimulator : MonoBehaviour
{
    /// <summary>
    /// 模拟信息模型的行为，每次tick都传播三轮，顺序/成功率是离散的，可能每一轮数值不一样
    /// </summary>
    
    public static InformationModelSimulator instance;

    Dictionary<int, Vector2> npcPositions;
    List<Character> allCharacters;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        allCharacters = CharacterManager.instance.GetCharacterList();
    }

    public void InformationSimStart()
    {
        // 默认是sim三轮
        // 每个小时都需要instantiate所有地图，然后进行模拟，全部结束后销毁，只留数据，然后创建最新的地图

        List<string> scenes = PhysicalWorldManager.instance.scenes;
        List<string> generatedScenes = new List<string>();

        HashSet<string> initiallyLoaded = new HashSet<string>();
        int sceneCount = SceneManager.sceneCount;
        for (int i = 0; i < sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.IsValid() && s.isLoaded)
            {
                initiallyLoaded.Add(s.name);
            }
        }

        foreach (string sceneName in scenes)
        {
            if (!initiallyLoaded.Contains(sceneName))
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                generatedScenes.Add(sceneName);
            }
        }

        SingleTick();

        foreach (string sceneName in generatedScenes)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }

    private void SingleTick()
    {
        // 每个房间内的NPC进行信息传播
        List<string> scenes = PhysicalWorldManager.instance.scenes;
        foreach (string sceneName in scenes)
        {
            SingleRoom(sceneName);
        }
    }

    private void SingleRoom(string roomName)
    {
        // 1. 构建NPC之间的距离图
        // NPCInstantiate npcInstantiate = GameObject.FindObjectOfType<NPCInstantiate>(); // 可能会出现的BUG：找到其他房间的instance

        Scene scene = SceneManager.GetSceneByName(roomName);
        if (!scene.IsValid() || !scene.isLoaded)
        {
            return;
        }

        NPCInstantiate npcInstantiate = null;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            npcInstantiate = root.GetComponentInChildren<NPCInstantiate>();
            if (npcInstantiate != null)
                break;
        }

        if (npcInstantiate == null)
        {
            return;
        }

        List<GameObject> npcsInScene = npcInstantiate.GetNPCInScene();
        npcPositions = new Dictionary<int, Vector2>();

        foreach (GameObject npcObj in npcsInScene)
        {
            CharacterInGame character = npcObj.GetComponent<CharacterInGame>(); // 没有。
            if (character == null) continue;

            int id = character.id;
            Vector2 position = new Vector2(npcObj.transform.position.x, npcObj.transform.position.y);
            npcPositions[id] = position;
        }
       
        foreach (GameObject npc in npcsInScene)
        {
            // 要有个锁定对象的toggle
            SingleSpread(npc);
        }
    }

    private void SingleSpread(GameObject npcObj)
    {
        CharacterInGame spreaderInGame = npcObj.GetComponent<CharacterInGame>();
        if (spreaderInGame == null) return;

        // 对于每个NPC，让它和周围的人进行交流，首先用influence值确认传播半径里都有哪些npc
        int spreaderId = spreaderInGame.id;
        Character spreader = allCharacters.Find(c => c != null && c.GetId() == spreaderId);
        if (spreader == null) return;

        if (!npcPositions.TryGetValue(spreaderId, out Vector2 spreaderPosition))
            return;

        float influenceRadius = spreader.GetInfluence();
        Dictionary<Character, int> nearbyCharacters = new Dictionary<Character, int>();
        foreach (Character character in allCharacters)
        {
            if (character == null) continue;
            if (character.GetId() == spreaderId) continue; // 跳过自己

            if (!npcPositions.TryGetValue(character.GetId(), out Vector2 characterPosition))
                continue;

            if (Vector2.Distance(spreaderPosition, characterPosition) <= influenceRadius * 2) // 影响范围是影响力*2
            {
                nearbyCharacters.Add(character, 1); // TODO: 这里应该用spreader对character的好感度来决定权重
            }
        }

        // 接着加权随机挑选传播对象，npc对其他人有好感度（默认为1），好感度越高越容易被随机数选中
        if (nearbyCharacters.Count == 0)
            return;

        Character chosenCharacter = null;
        int totalWeight = 0;
        foreach (var entry in nearbyCharacters)
        {
            totalWeight += entry.Value;
        }

        if (totalWeight <= 0)
            return;

        int randomValue = Random.Range(0, totalWeight);
        foreach (var entry in nearbyCharacters)
        {
            if (randomValue < entry.Value)
            {
                chosenCharacter = entry.Key;
                break;
            }
            randomValue -= entry.Value;
        }

        if (chosenCharacter == null)
            return;

        // 选中后，被选中的交流对象也算作“已经交流了”，在本tick只会和这个npc进行双向的交流，无法选中别人
        chosenCharacter.SetInteractionTarget(spreader);
        spreader.SetInteractionTarget(chosenCharacter);

        // 如果没有要传播的内容，结束
        if (string.IsNullOrEmpty(spreader.GetSpreadingIntel())) return;

        // 如果有要传播的内容，先看看对面是否已经知道，如果知道则结束
        List<string> chosenKnownIntel = chosenCharacter.GetKnownIntel();
        if (chosenKnownIntel.Contains(spreader.GetSpreadingIntel())) return;

        // 否则计算成功率：P=基础成功率X传播者的powerX被传播者是否感兴趣（是就x2）>预期则成功
        float basicSuccessRate = 0.1f; // TODO: 读表
        string tag = "example tag"; // TODO: 读表，传播内容对应的tag
        float successful = basicSuccessRate * spreader.GetPower() * (chosenCharacter.IsInterested(tag) ? 2.0f : 1.0f);
        if (Random.value > successful) return; // 失败则结束

        // 记录传播内容： 如果传播成功，把内容放入被传播者的存储里
        chosenCharacter.SetKnowTopic(spreader.GetSpreadingIntel());

        // 好感度更新，如果传播成功，双方好感度+1

    }
    
}
