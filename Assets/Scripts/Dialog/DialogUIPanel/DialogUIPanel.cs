using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogUIPanel : MonoBehaviour
{
    protected Image illustration;
    protected TMP_Text dialogText;
    protected TMP_Text nameText;
    
    protected void Awake()
    {
        illustration = FindDeepChild(transform, "Illustration").GetComponent<Image>();
        dialogText = FindDeepChild(transform, "DialogText").GetComponent<TMP_Text>();
        nameText = FindDeepChild(transform, "NameText").GetComponent<TMP_Text>();
    }

    /// <summary>
    /// 将对话框的内容设置为指定的内容，是一个抽象整合方法
    /// 目前如果立绘和名字相同则不会更新
    /// 特殊名字的处理放在子类中
    /// </summary>
    /// <param name="illustration"></param>
    /// <param name="name"></param>
    /// <param name="dialog"></param>
    public virtual void SetDialog(Sprite illustration, string name, string dialog)
    {
        SetIllustration(illustration);
        SetNameText(name);
        SetDialogText(dialog);
    }

    protected Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            var result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    protected void SetIllustration(Sprite sprite)
    {
        if(sprite == illustration.sprite){
            return;
        }
        illustration.sprite = sprite;
    }

    protected void SetDialogText(string text)
    {
        dialogText.text = text + "<?waitInput>"; // 顺应Febucci的标签
    }

    protected void SetNameText(string text)
    {
        if(text == nameText.text){
            return;
        }
        nameText.text = text;
    }

    public void SetNameColor(Color color)
    {
        nameText.color = color;
    }
}
