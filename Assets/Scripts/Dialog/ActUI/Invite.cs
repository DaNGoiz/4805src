using UnityEngine;
using UnityEngine.UI;

public class Invite : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnInviteButtonClicked);

    }

    private void OnInviteButtonClicked()
    {

    }

}