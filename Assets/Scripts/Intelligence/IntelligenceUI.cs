using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using cfg;
using System.IO;
using SimpleJSON;
using System;

public class IntelligenceUI : MonoBehaviour
{
    public int intelligenceId;

    private Button button;
    private Image intelligenceImage;
    private bool isUnlocked = true; // 暂时
    private bool isFinished = true; // 暂时

    private string intelligenceName;
    private string intelligenceDescription;
    private string intelligenceImagePath;
    private string intelligenceTag;
    private List<string> intelligenceClues = new List<string>();
    public List<GameObject> clueObjects = new List<GameObject>(); // 暂时直接拖拽

    private cfg.intelligence.intelligence_map intelligenceData;
    private cfg.intelligence.Tbintelligence_map intelligenceMap;


    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        intelligenceImage = transform.Find("IntImage").GetComponent<Image>();

        intelligenceMap = new cfg.intelligence.Tbintelligence_map(
            JSON.Parse(Resources.Load<TextAsset>("Datas/Config/intelligence_tbintelligence_map").text)
        );
        intelligenceData = intelligenceMap.Get(intelligenceId);
        
        LoadIntelligenceData();
    }

    private void OnButtonClick()
    {
        if (isFinished)
        {
            // 可以加载情报具体信息
            Messenger.Broadcast<string, string, string, string>("LoadIntelligenceDetail", 
                intelligenceName, intelligenceDescription, intelligenceImagePath, intelligenceTag);
        }
    }

    private void LoadIntelligenceData()
    {
        intelligenceName = intelligenceData.Name;
        intelligenceDescription = intelligenceData.DescStart;
        intelligenceImagePath = intelligenceData.ImagePath;
        intelligenceImage.sprite = Resources.Load<Sprite>("Textures/Intelligence/" + intelligenceImagePath);
        intelligenceTag = intelligenceData.Tag;

        if (!string.IsNullOrEmpty(intelligenceData.Clues))
        {
            string[] clueIds = intelligenceData.Clues.Split(',');
            foreach (string clueId in clueIds)
            {
                intelligenceClues.Add(clueId);
            }
        }

        // 获取到线索，有多少个线索就加载多少个
        for (int i = 0; i < intelligenceClues.Count; i++)
        {
            clueObjects[i].SetActive(true);
        }
    }

    public string GetIntelligenceName()
    {
        return intelligenceName;
    }
}
