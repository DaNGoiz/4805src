using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ChoiceUIPanel : MonoBehaviour
{
    private Image background;
    private GameObject choiceButton; // from prefab

    private bool immediate = false;

    [Header("Layout Settings")]
    public float buttonSpacing = 30f;
    public float baseY = 20f;
    public Vector2 buttonStartOffset = new Vector2(0, -80f); // 从下偏移进入
    public Vector2 buttonAnchorPosition = new Vector2(80, 0); // X位置
    public float backgroundTopPadding = 20f; // 背景顶部额外间距

    [Header("Background Animation")]
    public float backgroundExpandDuration = 0.5f;
    public Ease backgroundEase = Ease.OutCubic;

    [Header("Button Animation")]
    public float buttonFadeDuration = 0.4f;
    public float buttonMoveDuration = 0.5f;
    public float buttonStaggerDelay = 0.1f;
    public Ease buttonMoveEase = Ease.OutBack;
    public Ease buttonFadeEase = Ease.Linear;


    void Start()
    {
        background = transform.Find("SelectionPanel").GetComponent<Image>();
        choiceButton = Resources.Load<GameObject>("Prefabs/ChoiceButton");

        Messenger.AddListener<List<string>>(MsgType.ShowChoices, DisplayChoices);
        Messenger.AddListener<int>(MsgType.PlayerChoiceResult, PlayHideAnimation);

        immediate = true;
        PlayHideAnimation(-1); // 初始隐藏
    }

    private List<GameObject> buttonList = new List<GameObject>();

    private void DisplayChoices(List<string> choices)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject != background.gameObject)
                Destroy(child.gameObject);
        }

        Messenger.Broadcast(MsgType.PlayerChoosing, -1);

        RectTransform bgRect = background.rectTransform;
        bgRect.anchorMin = new Vector2(0.5f, 0);
        bgRect.anchorMax = new Vector2(0.5f, 0);
        bgRect.pivot = new Vector2(0.5f, 0);
        bgRect.anchoredPosition = new Vector2(bgRect.anchoredPosition.x, 0);

        float totalHeight = baseY + buttonSpacing * choices.Count + backgroundTopPadding;

        buttonList.Clear();

        for (int i = choices.Count - 1; i >= 0; i--)
        {
            GameObject button = Instantiate(choiceButton, transform);
            RectTransform rect = button.GetComponent<RectTransform>();
            CanvasGroup canvas = button.GetComponent<CanvasGroup>() ?? button.AddComponent<CanvasGroup>();

            rect.anchorMin = new Vector2(0.5f, 0);
            rect.anchorMax = new Vector2(0.5f, 0);
            rect.pivot = new Vector2(0.5f, 0);

            float yPos = baseY + buttonSpacing * (choices.Count - 1 - i);
            Vector2 finalPos = new Vector2(buttonAnchorPosition.x, yPos);
            Vector2 startPos = finalPos + buttonStartOffset;

            rect.anchoredPosition = startPos;
            canvas.alpha = 0;

            button.GetComponent<ChoiceButton>().index = i;
            button.GetComponentInChildren<TextMeshProUGUI>().text = choices[i];

            buttonList.Add(button);
        }


        AnimateWithDOTween(buttonList, totalHeight);
    }

    private void AnimateWithDOTween(List<GameObject> buttons, float targetBgHeight)
    {
        RectTransform bgRect = background.rectTransform;
        CanvasGroup bgCanvas = background.GetComponent<CanvasGroup>() ?? background.gameObject.AddComponent<CanvasGroup>();

        // 背景动画
        bgCanvas.alpha = 0;
        bgRect.sizeDelta = new Vector2(bgRect.sizeDelta.x, 0);
        bgCanvas.DOFade(1f, backgroundExpandDuration * 0.8f);
        bgRect.DOSizeDelta(new Vector2(bgRect.sizeDelta.x, targetBgHeight), backgroundExpandDuration)
            .SetEase(backgroundEase);

        // 按钮动画
        for (int i = 0; i < buttons.Count; i++)
        {
            var btn = buttons[i];
            var rect = btn.GetComponent<RectTransform>();
            var canvas = btn.GetComponent<CanvasGroup>();

            Vector2 endPos = new Vector2(buttonAnchorPosition.x, baseY + buttonSpacing * i);

            rect.DOAnchorPos(endPos, buttonMoveDuration)
                .SetEase(buttonMoveEase)
                .SetDelay(i * buttonStaggerDelay);

            canvas.DOFade(1f, buttonFadeDuration)
                .SetEase(buttonFadeEase)
                .SetDelay(i * buttonStaggerDelay);
        }
    }

    private void PlayHideAnimation(int idx)
    {
        float duration = 0.25f;
        float moveOffsetY = -60f;
        Ease ease = Ease.InBack;

        // 背景动画
        RectTransform bgRect = background.rectTransform;
        CanvasGroup bgCanvas = background.GetComponent<CanvasGroup>();

        bgCanvas.DOFade(0f, duration * 0.8f * (immediate ? 0f : 1f));
        bgRect.DOSizeDelta(new Vector2(bgRect.sizeDelta.x, 0f), duration)
            .SetEase(ease);

        // 所有按钮动画
        foreach (GameObject button in buttonList)
        {
            if (button == null) continue;

            RectTransform rect = button.GetComponent<RectTransform>();
            CanvasGroup canvas = button.GetComponent<CanvasGroup>();

            rect.DOAnchorPosY(rect.anchoredPosition.y + moveOffsetY, duration)
                .SetEase(ease);

            canvas.DOFade(0f, duration * 0.8f * (immediate ? 0f : 1f));
        }
        immediate = false;
    }
}