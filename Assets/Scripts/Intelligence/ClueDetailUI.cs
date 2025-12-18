using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClueDetailUI : MonoBehaviour
{
    private TextMeshProUGUI clueNameText;
    private TextMeshProUGUI clueDescriptionText;
    private Image clueImage;

    void Start()
    {
        clueNameText = transform.Find("Background/Title").GetComponent<TextMeshProUGUI>();
        clueDescriptionText = transform.Find("Background/Description").GetComponent<TextMeshProUGUI>();
        clueImage = transform.Find("Background/Image").GetComponent<Image>();

        Messenger.AddListener<string, string, string>("LoadClueDetail", LoadDetailData);
        Messenger.AddListener<string, string, string, string>("LoadIntelligenceDetail", BackgroundSetActive);
    }

    private void LoadDetailData(string name, string description, string imagePath)
    {
        transform.Find("Background").gameObject.SetActive(true);
        clueNameText.text = name;
        clueDescriptionText.text = description;
        clueImage.sprite = Resources.Load<Sprite>("Textures/Clues/" + imagePath);
    }

    private void BackgroundSetActive(string a, string b, string c, string d)
    {
        transform.Find("Background").gameObject.SetActive(false);
    }
}
