using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IntDetailUI : MonoBehaviour
{
    private TextMeshProUGUI intelligenceNameText;
    private TextMeshProUGUI intelligenceDescriptionText;
    private Image intelligenceImage;
    private string intelligenceTag;


    void Start()
    {
        intelligenceNameText = transform.Find("Background/Title").GetComponent<TextMeshProUGUI>();
        intelligenceDescriptionText = transform.Find("Background/Description").GetComponent<TextMeshProUGUI>();
        intelligenceImage = transform.Find("Background/Image").GetComponent<Image>();

        Messenger.AddListener<string, string, string, string>("LoadIntelligenceDetail", LoadDetailData);
    }

    private void LoadDetailData(string name, string description, string imagePath, string tag)
    {
        intelligenceNameText.text = name;
        intelligenceDescriptionText.text = description;
        intelligenceTag = tag;
        intelligenceImage.sprite = Resources.Load<Sprite>("Textures/Intelligence/" + imagePath);
    }
}
