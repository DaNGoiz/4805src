using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUIToggle : MonoBehaviour
{
    private GameObject mapUI;

    void Start()
    {
        mapUI = transform.Find("Background").gameObject;
        mapUI.SetActive(false);

        Messenger.AddListener(MsgType.CloseMap, CloseMap);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(MsgType.CloseMap, CloseMap);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMapUI();
        }
    }

    private void ToggleMapUI()
    {
        if (mapUI.activeSelf)
        {
            CloseMap();
        }
        else
        {
            OpenMap();
        }
    }

    private void OpenMap()
    {
        mapUI.SetActive(true);
        Time.timeScale = 0f;
    }

    private void CloseMap()
    {
        if (!mapUI.activeSelf) return;

        mapUI.SetActive(false);
        Time.timeScale = 1f;
    }
}
