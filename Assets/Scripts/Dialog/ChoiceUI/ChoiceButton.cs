using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChoiceButton : MonoBehaviour
{
    private TextMeshProUGUI text;
    private GameObject chosenEffect;
    private AudioSource audioSource;
    private Button button;
    private bool isChosen = false;
    public int index;
    

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        chosenEffect = transform.Find("Selected").gameObject;
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        audioSource = GetComponent<AudioSource>();

        EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((eventData) => { OnPointerEnter(); });
        eventTrigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => { OnPointerExit(); });
        eventTrigger.triggers.Add(entryExit);

        Messenger.AddListener<int>(MsgType.PlayerChoosing, OnPlayerChoosing);
    }

    private void OnButtonClick()
    {
        if (!isChosen)
        {
            isChosen = true;
            // text.color = Color.yellow;
            chosenEffect.SetActive(true);
            chosenEffect.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            Messenger.Broadcast(MsgType.PlayerChoosing, index);
        }
        if (isChosen)
        {
            Messenger.Broadcast(MsgType.PlayerChoiceResult, index);
            Messenger.Broadcast(MsgType.PlayerChoosing, -1); // 重置选择状态
            button.interactable = false; // 防止重复点击
            audioSource.Play();
        }
    }

    private void OnPlayerChoosing(int index)
    {
        if (this.index != index)
        {
            isChosen = false;
            chosenEffect.SetActive(false);
            text.color = Color.white;
        }
    }

    private void OnPointerEnter()
    {
        if (!isChosen)
        {
            chosenEffect.SetActive(true);
            chosenEffect.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }
    }

    private void OnPointerExit()
    {
        if (!isChosen)
        {
            chosenEffect.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<int>(MsgType.PlayerChoosing, OnPlayerChoosing);
        button.onClick.RemoveAllListeners();
        if (chosenEffect != null)
        {
            Destroy(chosenEffect);
        }
        if (text != null)
        {
            Destroy(text);
        }
        if (button != null)
        {
            Destroy(button);
        }
    }
}