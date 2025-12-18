using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using cfg;
using System.IO;
using SimpleJSON;
using System;

public class ClueUI : MonoBehaviour
{
    private Button button;
    private bool isUnlocked = false; // 暂时

    public int clueId;
    private string clueName;
    private string clueDescription;
    private string clueImagePath;

    private cfg.intelligence.clue_map clueData;
    private cfg.intelligence.Tbclue_map clueMap;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);

        clueMap = new cfg.intelligence.Tbclue_map(
            JSON.Parse(Resources.Load<TextAsset>("Datas/Config/intelligence_tbclue_map").text)
        );
        LoadClueData(clueId);

        Messenger.AddListener<int>("OnWorldClueClicked", (id) => {
            if (id == clueId)
            {
                isUnlocked = true;
                Image image = GetComponent<Image>();
                image.sprite = Resources.Load<Sprite>("Textures/Clue/" + clueImagePath);
            }
        });
    }

    private void OnButtonClick()
    {
        if (isUnlocked)
        {
            Messenger.Broadcast<string, string, string>("LoadClueDetail", 
                clueName, clueDescription, clueImagePath);
        }
    }

    private void LoadClueData(int id)
    {
        clueData = clueMap.Get(id);
        clueName = clueData.Name;
        clueDescription = clueData.Description;
        clueImagePath = clueData.ImagePath;
    }
}
