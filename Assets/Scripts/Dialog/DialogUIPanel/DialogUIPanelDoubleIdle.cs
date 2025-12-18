using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DialogUIPanelDoubleIdle : DialogUIPanel
{
    private Image imageLeft;
    private Image imageRight;

    private Vector3 originalScaleLeft;
    private Vector3 originalScaleRight;
    private Vector2 originalPosLeft;
    private Vector2 originalPosRight;
    private Vector2 originalSizeLeft;
    private Vector2 originalSizeRight;

    private string previousSide = "";
    private Sprite previousSpriteLeft = null;
    private Sprite previousSpriteRight = null;

    private Dictionary<string, string> nameToSide = new(); // 记录角色名与方向

    private void Awake()
    {
        dialogText = FindDeepChild(transform, "DialogText").GetComponent<TMP_Text>();
        nameText = FindDeepChild(transform, "NameText").GetComponent<TMP_Text>();
        imageLeft = FindDeepChild(transform, "IllustrationLeft").GetComponent<Image>();
        imageRight = FindDeepChild(transform, "IllustrationRight").GetComponent<Image>();

        FixPivotCenter(imageLeft.rectTransform);
        FixPivotCenter(imageRight.rectTransform);

        // 记录初始信息
        originalScaleLeft = imageLeft.rectTransform.localScale;
        originalScaleRight = imageRight.rectTransform.localScale;
        originalPosLeft = imageLeft.rectTransform.anchoredPosition;
        originalPosRight = imageRight.rectTransform.anchoredPosition;
        originalSizeLeft = imageLeft.rectTransform.sizeDelta;
        originalSizeRight = imageRight.rectTransform.sizeDelta;
    }

    public void SetDialog(Sprite illustration, string name, string dialog, string illustDirection = "left")
    {
        if (nameToSide.TryGetValue(name, out string rememberedSide))
        {
            illustDirection = rememberedSide;
        }

        bool isRight = illustDirection == "r" || illustDirection == "right";
        Image speakingImage = isRight ? imageRight : imageLeft;
        Image silentImage = isRight ? imageLeft : imageRight;
        Sprite previousSprite = isRight ? previousSpriteRight : previousSpriteLeft;

        SetIllustration(illustration, speakingImage, isRight);
        SetNameText(name);
        SetDialogText(dialog);

        bool isNewPerson = speakingImage.sprite != previousSprite;
        if (isNewPerson || previousSide != illustDirection)
        {
            PlayBounce(speakingImage.rectTransform);
        }

        SetGrayscale(speakingImage, false);
        SetGrayscale(silentImage, true);

        if (isRight) previousSpriteRight = illustration;
        else previousSpriteLeft = illustration;

        previousSide = illustDirection;
        nameToSide[name] = illustDirection;

        string silentName = GetNameBySprite(silentImage.sprite);
        if (!string.IsNullOrEmpty(silentName) && silentImage.sprite != illustration)
        {
            nameToSide.Remove(silentName);
        }
    }

    private void SetIllustration(Sprite sprite, Image targetImage, bool isRight)
    {
        if (targetImage.sprite == sprite) return;

        // 强制补丁
        if (sprite == null) return;

        targetImage.sprite = sprite;
        targetImage.SetNativeSize(); // 会改变 sizeDelta

        // 恢复最初宽高
        if (isRight)
            targetImage.rectTransform.sizeDelta = originalSizeRight;
        else
            targetImage.rectTransform.sizeDelta = originalSizeLeft;
    }

    private void PlayBounce(RectTransform rect)
    {
        bool isLeft = rect == imageLeft.rectTransform;

        Vector3 originalScale = isLeft ? originalScaleLeft : originalScaleRight;
        Vector2 originalPos = isLeft ? originalPosLeft : originalPosRight;
        Vector2 originalSize = isLeft ? originalSizeLeft : originalSizeRight;

        rect.DOKill();
        rect.localScale = originalScale;
        rect.anchoredPosition = originalPos;
        rect.sizeDelta = originalSize;

        rect.DOScale(originalScale * 1.1f, 0.1f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

private void SetGrayscale(Image img, bool gray)
{
    // 使用 tag 控制只结束 grayscale 动画，保留 bounce
    string tweenTag = "GrayscaleTween_" + img.name;
    DOTween.Kill(tweenTag);

    bool isLeft = img == imageLeft;
    Vector3 originalScale = isLeft ? originalScaleLeft : originalScaleRight;

    Color currentColor = img.color;
    bool isCurrentlyGray = currentColor.r < 0.6f && currentColor.g < 0.6f && currentColor.b < 0.6f;

    if (gray)
    {
        if (!isCurrentlyGray)
        {
            img.DOColor(new Color(0.5f, 0.5f, 0.5f, currentColor.a), 0.3f)
                .SetEase(Ease.InOutSine)
                .SetId(tweenTag);

            img.rectTransform.DOScale(originalScale * 0.95f, 0.3f)
                .SetEase(Ease.OutSine)
                .SetId(tweenTag);
        }
    }
    else
    {
        img.color = new Color(1f, 1f, 1f, currentColor.a);
        img.rectTransform.localScale = originalScale;
    }
}

    private void FixPivotCenter(RectTransform rect)
    {
        rect.pivot = new Vector2(0.5f, 0.5f); // 适配居中锚点
    }

    private void UpdateOriginalSize(Image img)
    {
        if (img == imageLeft)
            originalSizeLeft = img.rectTransform.sizeDelta;
        else if (img == imageRight)
            originalSizeRight = img.rectTransform.sizeDelta;
    }

    private string GetNameBySprite(Sprite sprite)
    {
        foreach (var kvp in nameToSide)
        {
            if ((kvp.Value == "left" && previousSpriteLeft == sprite) ||
                (kvp.Value == "right" && previousSpriteRight == sprite))
            {
                return kvp.Key;
            }
        }
        return null;
    }
}
