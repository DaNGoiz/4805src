using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateManager : MonoBehaviour
{
    public static DateManager instance;
    public int Day { get; private set; }
    public int Hour { get; private set; }

    private int currentSimAccumulator = 0; // 用来累计小时数，每3小时触发一次传播模拟

    void Awake()
    {
        instance = this;

        Day = 1;
        Hour = 18;   // 初始 Day1 18:00

        Messenger.AddListener(MsgType.SaveGame, SaveGame);
    }

    public void AddHour(int addHrAmt)
    {
        for (int i = 0; i < addHrAmt; i++)
        {
            Hour += 1;
            currentSimAccumulator += 1;

            if (Hour >= 24)
            {
                Hour -= 24;
                Day += 1;
            }

            // 每三个小时只需要叫一次sim
            if (currentSimAccumulator >= 3)
            {
                InformationModelSimulator.instance.InformationSimStart();
                currentSimAccumulator = 0;
            }
        }
    }

    private void SaveGame()
    {
        PlayerPrefs.SetInt("Day", Day);
        PlayerPrefs.SetInt("Hour", Hour);
        PlayerPrefs.Save();
    }
}
