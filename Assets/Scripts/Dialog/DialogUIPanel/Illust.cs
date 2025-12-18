using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Illust : MonoBehaviour
{
    private Image image;
    private Febucci.UI.TextAnimatorPlayer textAnimatorPlayer;

    void Start()
    {
        image = GetComponent<Image>();
        textAnimatorPlayer = FindObjectOfType<Febucci.UI.TextAnimatorPlayer>();
        textAnimatorPlayer.textAnimator.onEvent += OnEvent;
    }

    private void OnEvent(string message)
    {
        if (message.StartsWith("IllustShake"))
        {
            StartCoroutine(IllustShakeCoroutine());
        }
        else if (message.StartsWith("IllustFadeOut"))
        {
            StartCoroutine(IllustFadeOutCoroutine());
        }
        else if (message.StartsWith("IllustFadeIn"))
        {
            StartCoroutine(IllustFadeInCoroutine());
        }
        else if (message.StartsWith("IllustRunAway"))
        {
            StartCoroutine(IllustRunAwayCoroutine());
        }
    }

    private IEnumerator IllustShakeCoroutine()
    {
        Vector2 originPos = image.rectTransform.anchoredPosition;
        for (int i = 0; i < 5; i++)
        {
            image.rectTransform.anchoredPosition = new Vector2(Random.Range(-5, 5) + originPos.x, Random.Range(-5, 5) + originPos.y);
            yield return new WaitForSeconds(0.05f);
        }
        image.rectTransform.anchoredPosition = originPos;
    }

    private IEnumerator IllustFadeOutCoroutine()
    {
        float duration = 0.5f;
        float time = 0;
        Color originColor = image.color;
        while (time < duration)
        {
            time += Time.deltaTime;
            image.color = Color.Lerp(originColor, new Color(1, 1, 1, 0), time / duration);
            yield return null;
        }
        image.color = new Color(1, 1, 1, 0);

        image.sprite = Resources.Load<Sprite>("Textures/Dialog/Illustrations/Blank/Blank_Default");
        image.color = new Color(1, 1, 1, 1);
    }

    private IEnumerator IllustFadeInCoroutine()
    {
        float duration = 0.5f;
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            image.color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), time / duration);
            yield return null;
        }
        image.color = new Color(1, 1, 1, 1);
    }

    private IEnumerator IllustRunAwayCoroutine()
    {
        Vector2 originPos = image.rectTransform.anchoredPosition;
        float duration = 0.5f;
        float time = 0;
        Vector2 targetPos = new Vector2(1000, 0);
        while (time < duration)
        {
            time += Time.deltaTime;
            image.rectTransform.anchoredPosition = Vector2.Lerp(originPos, targetPos, time / duration);
            yield return null;
        }
        image.rectTransform.anchoredPosition = targetPos;
        image.sprite = Resources.Load<Sprite>("Textures/Dialog/Illustrations/Blank/Blank_Default");
        image.rectTransform.anchoredPosition = originPos;
    }
}
