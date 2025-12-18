using System.Collections.Generic;
using UnityEngine;

public class NPCInstantiate : MonoBehaviour
{
    private List<GameObject> npcInScene = new List<GameObject>();

    private cfg.story.Tbnpc_basic_info npcMap;

    public string placeName;

    [SerializeField] private Transform characterRoot;

    void Awake()
    {
        npcMap = new cfg.story.Tbnpc_basic_info(
            SimpleJSON.JSON.Parse(Resources.Load<TextAsset>("Datas/Config/story_tbnpc_basic_info").text)
        );

        if (characterRoot == null)
        {
            GameObject rootObj = GameObject.Find("CharacterRoot");
            if (rootObj != null)
                characterRoot = rootObj.transform;
        }
    }

    void Start()
    {
        int initTime = 18;
        if (DateManager.instance != null)
        {
            initTime = GetTimeSlotFromHour(DateManager.instance.Hour);
        }

        RefreshNPCsForTime(initTime);

        Messenger.AddListener(MsgType.Travel, OnTimeChanged);
        Messenger.AddListener(MsgType.Nap, OnTimeChanged);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(MsgType.Travel, OnTimeChanged);
        Messenger.RemoveListener(MsgType.Nap, OnTimeChanged);
    }

    private void OnTimeChanged()
    {
        if (DateManager.instance == null) return;

        int timeSlot = GetTimeSlotFromHour(DateManager.instance.Hour);
        RefreshNPCsForTime(timeSlot);
    }

    public void RefreshNPCsForTime(int time)
    {
        ClearAllNPCs();
        InstantiateNPCInScene(time);
    }

    private int GetTimeSlotFromHour(int hour)
    {
        int slot = (hour / 3) * 3;
        if (slot < 0) slot = 0;
        if (slot > 21) slot = 21;
        return slot;
    }

    private void ClearAllNPCs()
    {
        foreach (var npc in npcInScene)
        {
            if (npc != null)
            {
                Destroy(npc);
            }
        }
        npcInScene.Clear();
    }

    private void InstantiateNPCInScene(int time)
    {
        if (characterRoot == null)
        {
            Debug.LogWarning("[NPCInstantiate] CharacterRoot is null, cannot spawn NPCs.");
            return;
        }

        // 1. 读取预设点位（CharacterRoot 的所有子物体）
        List<Transform> spawnPoints = new List<Transform>();
        foreach (Transform child in characterRoot)
        {
            spawnPoints.Add(child);
        }

        // 2. 获取当前在这个房间里的 npc 的编号
        List<int> availableNPCIds = new List<int>();
        int NPCCount = PhysicalWorldManager.NPCCount;

        for (int i = 0; i < NPCCount; i++)
        {
            var npcData = npcMap.Get(i);
            if (npcData != null)
            {
                string place = GetNPCPlaceByTime(time, i);
                if (!string.IsNullOrEmpty(place) && place == placeName)
                {
                    availableNPCIds.Add(i);
                }
            }
        }

        // 3. 随机分配到点位，并“以 point 为 root”生成
        System.Random rand = new System.Random();
        foreach (int npcId in availableNPCIds)
        {
            if (spawnPoints.Count == 0)
                break;

            int pointIndex = rand.Next(spawnPoints.Count);
            Transform spawnPoint = spawnPoints[pointIndex];
            spawnPoints.RemoveAt(pointIndex);

            cfg.story.npc_basic_info npcInfo = npcMap.Get(npcId);
            string name = npcInfo.Name;

            GameObject npcPrefab = Resources.Load<GameObject>("Prefabs/Characters/" + name);
            if (npcPrefab == null)
            {
                Debug.LogWarning($"[NPCInstantiate] NPC prefab not found: Prefabs/Characters/{name}");
                continue;
            }

            GameObject npcObject = Instantiate(npcPrefab, spawnPoint);
            npcObject.transform.localPosition = Vector3.zero;

            npcInScene.Add(npcObject);
        }

        Debug.Log($"[NPCInstantiate] Spawned {npcInScene.Count} NPCs in place {placeName} at time {time}.");
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

    public List<GameObject> GetNPCInScene()
    {
        return npcInScene;
    }
}
