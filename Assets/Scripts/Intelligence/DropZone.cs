using UnityEngine;
using UnityEngine.UI;

public class DropZone : MonoBehaviour
{
    private Image targetImage;


    private void Start()
    {
        targetImage = GetComponent<Image>();
    }
    public void ReceiveDroppedSprite(Sprite sprite)
    {
        if (targetImage != null)
        {
            targetImage.sprite = sprite;
            targetImage.enabled = true;
        }
    }
}
