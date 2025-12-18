using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogBox : MonoBehaviour
{
    private Image bgImage;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI contentText;
    private Febucci.UI.TextAnimatorPlayer textAnimatorPlayer;

    private Vector2 originalSize;

    void Start()
    {
        bgImage = GetComponent<Image>();
        nameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        contentText = transform.Find("DialogText").GetComponent<TextMeshProUGUI>();
        textAnimatorPlayer = FindObjectOfType<Febucci.UI.TextAnimatorPlayer>();
        textAnimatorPlayer.textAnimator.onEvent += OnEvent;

        originalSize = bgImage.rectTransform.sizeDelta;
    }

    private void OnEvent(string message)
    {
        if (message.StartsWith("ShowDialog"))
        {
            StartCoroutine(ShowDialogCoroutine());
        }
        else if (message.StartsWith("CloseDialog"))
        {
            StartCoroutine(CloseDialogCoroutine());
        }
    }

    private IEnumerator ShowDialogCoroutine()
    {
        float duration = 0.3f;
        float time = 0;

        bgImage.rectTransform.sizeDelta = new Vector2(originalSize.x, 0);
        bgImage.color = new Color(1, 1, 1, 0);
        bgImage.enabled = true;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            bgImage.rectTransform.sizeDelta = new Vector2(originalSize.x, Mathf.Lerp(0, originalSize.y, t));
            bgImage.color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), t);

            yield return null;
        }

        bgImage.rectTransform.sizeDelta = originalSize;
        bgImage.color = new Color(1, 1, 1, 1);
    }

    private IEnumerator CloseDialogCoroutine()
    {
        nameText.text = "";
        contentText.text = "";
        
        float duration = 0.3f;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            bgImage.rectTransform.sizeDelta = new Vector2(originalSize.x, Mathf.Lerp(originalSize.y, 0, t));
            bgImage.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), t);

            yield return null;
        }

        bgImage.rectTransform.sizeDelta = new Vector2(originalSize.x, 0);
        bgImage.color = new Color(1, 1, 1, 0);
        bgImage.enabled = false;
    }
}