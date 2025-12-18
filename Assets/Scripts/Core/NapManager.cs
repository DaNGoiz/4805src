using UnityEngine;

public class NapManager : MonoBehaviour
{
    public int napTimeCost = 3;
    public int napEnergyRecover = 1;

    private void Awake()
    {
        Messenger.AddListener(MsgType.Nap, OnNap);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(MsgType.Nap, OnNap);
    }

    private void OnNap()
    {
        DateManager.instance.AddHour(napTimeCost);
        EnergyManager.instance.AddEnergy(napEnergyRecover);

        Debug.Log($"[NapManager] Nap: +{napTimeCost}h, +{napEnergyRecover} energy");
    }
}
