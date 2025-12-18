using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.UI;

public class PlaceUI : MonoBehaviour
{
    private Button button;
    private Image iconImage;

    public string PlaceName;

    [SerializeField]
    private bool isUnlocked;

    private bool isVisiting;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        Messenger.AddListener(MsgType.Travel, CheckVisitingPlace);

        if (PlaceName == "The Palace") isVisiting = true;

        iconImage = GetComponent<Image>();
        if (isVisiting)
        {
            iconImage.GetComponent<UIEffect>().enabled = true;
        }
        else
        {
            iconImage.GetComponent<UIEffect>().enabled = false;
        }
    }

    private void OnButtonClick()
    {
        if (!isVisiting)
        {
            Messenger.Broadcast(MsgType.PreviewTravel, PlaceName);
        }
        else
        {
            Messenger.Broadcast(MsgType.CancelPreviewTravel);
        }
    }

    private void CheckVisitingPlace()
    {
        string targetPlace = TravelManager.instance.TargetPlace;
        if (PlaceName == targetPlace)
        {
            isVisiting = true;
            iconImage.GetComponent<UIEffect>().enabled = true;
        }
        else
        {
            isVisiting = false;
            iconImage.GetComponent<UIEffect>().enabled = false;
        }
    }
}
