using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogUIPanelSingleIdle : DialogUIPanel
{
    private GameObject nameBG;

    void Awake()
    {
        base.Awake();
        nameBG = FindDeepChild(transform, "NameBG").gameObject;
    }

    public override void SetDialog(Sprite illustration, string name, string dialog)
    {
        base.SetDialog(illustration, name, dialog);
        SetNameBG(name);
    }

    private void SetNameBG(string name)
    {
        if(name == ""){
            nameBG.SetActive(false);
        }
        else {
            nameBG.SetActive(true);
        }
    }
}
