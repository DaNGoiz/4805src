using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using cfg;
using SimpleJSON;

public class TravelManager : MonoBehaviour
{
    public static TravelManager instance;
    public string CurrentPlace { get; private set; }
    public string TargetPlace { get; private set; }

    private int energyCost;
    private int timeCost;

    private cfg.map.Tbtravel_map placeMap;
    private cfg.map.travel_map placeData;

    private void Awake()
    {
        instance = this;
        
        Messenger.AddListener<string>(MsgType.PreviewTravel, OnPreviewTravel);
        Messenger.AddListener(MsgType.Travel, OnTravel);

        CurrentPlace = "The Palace";

        placeMap = new cfg.map.Tbtravel_map(
            JSON.Parse(Resources.Load<TextAsset>("Datas/Config/map_tbtravel_map").text)
        );
    }

    private void OnPreviewTravel(string placeName)
    {
        TargetPlace = placeName;

        placeData = placeMap.Get(CurrentPlace);

        if (placeName == "Garden")
        {
            energyCost = placeData.Garden;
            timeCost = placeData.Garden * 3;
        }
        else if (placeName == "Library")
        {
            energyCost = placeData.Library;
            timeCost = placeData.Library * 3;
        }
        else if (placeName == "The Palace")
        {
            energyCost = placeData.ThePalace;
            timeCost = placeData.ThePalace * 3;
        }
        
        Messenger.Broadcast(MsgType.PreviewTravelResult, placeName, energyCost, timeCost);
    }

    private void OnTravel()
{
    int currentEnergy = EnergyManager.instance.Energy;
    if (currentEnergy < energyCost)
    {
        return;
    }

    EnergyManager.instance.UseEnergy(energyCost);
    DateManager.instance.AddHour(timeCost);

    PhysicalWorldManager.instance.UnloadScene(CurrentPlace);
    PhysicalWorldManager.instance.LoadScene(TargetPlace);
    CurrentPlace = TargetPlace;

    Messenger.Broadcast(MsgType.CloseMap);

    Debug.Log($"[TravelManager] Travel to {TargetPlace}, cost {energyCost} energy, +{timeCost}h.");
}

}
