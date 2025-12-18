using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager instance;
    public int Energy { get; private set; }
    private int MaxEnergy = 6;

    private void Awake()
    {
        instance = this;

        Energy = MaxEnergy;

        // Messenger.AddListener(MsgType.NewGameInit, () =>
        // {
        //     Energy = MaxEnergy;
        // });
        Messenger.AddListener(MsgType.SaveGame, SaveGame);
    }

    public void AddEnergy(int AddEnergyAmt)
    {
        Energy += AddEnergyAmt;
        if (Energy > MaxEnergy)
        {
            Energy = MaxEnergy;
        }
    }

    public void UseEnergy(int UseEnergyAmt)
    {
        Energy -= UseEnergyAmt;
        if (Energy < 0)
        {
            Energy = 0;
        }
    }

    private void SaveGame()
    {
        PlayerPrefs.SetInt("Energy", Energy);
        PlayerPrefs.Save();
    }
    
    public void LoadGame()
    {
        Energy = PlayerPrefs.GetInt("Energy", MaxEnergy);
    }
}
