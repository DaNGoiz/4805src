using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DateEnergyUI : MonoBehaviour
{
    public TextMeshProUGUI dateText;
    public List<Image> energyImages = new List<Image>();

    private Color fullEnergyColor = new Color(0.83f, 0.64f, 0.26f);
    private Color lowEnergyColor = Color.gray;
    private Color previewEnergyColor = new Color(0.2f, 0.6f, 1f);

    private int previewDay;
    private int previewHour;
    private int previewEnergy;
    private int realDay;
    private int realHour;
    private int realEnergy;

    private Color normalDateColor;

    void Start()
    {
        normalDateColor = dateText.color;

        Messenger.AddListener<string, int, int>(MsgType.PreviewTravelResult, PreviewUpdate);

        // 真正执行移动
        Messenger.AddListener(MsgType.Travel, OnRealChanged);
        Messenger.AddListener(MsgType.Nap, OnRealChanged);

        // 取消预览
        Messenger.AddListener(MsgType.CancelPreviewTravel, OnCancelPreview);

        RealUpdate();
    }

    void OnDestroy()
    {
        // Messenger.RemoveListener<string, int, int>(MsgType.PreviewTravelResult, PreviewUpdate);
        // Messenger.RemoveListener(MsgType.Travel, OnRealChanged);
        // Messenger.RemoveListener(MsgType.Nap, OnRealChanged);
        // Messenger.RemoveListener(MsgType.CancelPreviewTravel, OnCancelPreview);
    }

    private void OnRealChanged()
    {
        previewDay = 0;
        previewHour = 0;
        previewEnergy = 0;

        RealUpdate();
    }

    private void OnCancelPreview()
    {
        previewDay = 0;
        previewHour = 0;
        previewEnergy = 0;

        RealUpdate();
    }

    private void RealUpdate()
    {
        realDay = DateManager.instance.Day;
        realHour = DateManager.instance.Hour;
        realEnergy = EnergyManager.instance.Energy;

        UpdateDate(realDay, realHour);
        UpdateEnergy(realEnergy);

        dateText.color = normalDateColor;
    }

    private void UpdateDate(int day, int hour)
    {
        // 显示格式：Day 1 - 18:00
        dateText.text = $"Day {day} - {hour:00}:00";
    }

    private void UpdateEnergy(int energy)
    {
        for (int i = 0; i < energyImages.Count; i++)
        {
            if (i < energy)
            {
                energyImages[i].color = fullEnergyColor;
            }
            else
            {
                energyImages[i].color = lowEnergyColor;
            }
        }
    }

    private void PreviewUpdate(string placeName, int energyCost, int timeCost)
    {
        List<int> previewData = PreviewDateAndEnergy(energyCost, timeCost);
        previewDay = previewData[0];
        previewHour = previewData[1];
        previewEnergy = previewData[2];

        int currentRealEnergy = EnergyManager.instance.Energy;

        for (int i = 0; i < energyImages.Count; i++)
        {
            if (i < previewEnergy)
            {
                energyImages[i].color = fullEnergyColor;
            }
            else if (i < currentRealEnergy)
            {
                energyImages[i].color = previewEnergyColor;
            }
            else
            {
                energyImages[i].color = lowEnergyColor;
            }
        }

        dateText.text = $"Day {previewDay} - {previewHour:00}:00";
        dateText.color = new Color(0.2f, 0.6f, 1f);
    }

    private List<int> PreviewDateAndEnergy(int energyCost, int timeCost)
    {
        int currentDay = DateManager.instance.Day;
        int currentHour = DateManager.instance.Hour;
        int currentEnergy = EnergyManager.instance.Energy - energyCost;

        if (currentEnergy < 0) currentEnergy = 0;

        currentHour += timeCost;
        if (currentHour >= 24)
        {
            currentDay += currentHour / 24;
            currentHour = currentHour % 24;
        }

        return new List<int> { currentDay, currentHour, currentEnergy };
    }
}
