using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using cfg;
using System.IO;
using SimpleJSON;
using System;

public class DialogPlayer : MonoBehaviour
{
    private DialogUIPanelDoubleIdle dialogUIPanel; // 测试用，原本是DialogUIPanel
    private Febucci.UI.TextAnimatorPlayer textAnimatorPlayer;

    private Sprite illustration;
    private string characterName;
    private string dialogText;
    private cfg.story.dialog dialogLine;
    private cfg.Tables dialogTables;

    public bool autoPlay;
    public float autoPlayWaitTime;

    private bool hasNext;
    private bool isAwaitingInput;
    private bool isInterrupted; // 使用Messenger进行中断
    private bool isCoolingDown;
    private float coolDownTime = 0.5f;
    private bool isChoosing;
    private List<string> choiceBranchs; // 用于存储选择对应的故事分支
    private bool hasJump = false;
    private int jumpBranchLineIndex = -1;
    private int currentLineIndex;

    void Start()
    {
        dialogUIPanel = FindObjectOfType<DialogUIPanelDoubleIdle>();

        textAnimatorPlayer = FindObjectOfType<Febucci.UI.TextAnimatorPlayer>();
        textAnimatorPlayer.textAnimator.onEvent += OnEvent;

        Messenger.AddListener<bool>(MsgType.InterruptDialog, isInterrupted => this.isInterrupted = isInterrupted);
        Messenger.AddListener<bool>(MsgType.AutoPlay, autoPlay => this.autoPlay = autoPlay);
        Messenger.AddListener(MsgType.PlayerInputNext, HandleInput);
        Messenger.AddListener<int>(MsgType.DialogPlayStory, PrepareDialog);
        Messenger.AddListener<int>(MsgType.PlayerChoiceResult, HandleChoice);

        // 备注：有文件分类问题，目前暂时用tables读了所有的表
        // 以后改包了也会改路径
        // charIllustMap = new cfg.Tables(file => JSON.Parse(Resources.Load<TextAsset>($"Datas/Config/demo_tbdialogmap").text));
        dialogTables = new cfg.Tables(file => JSON.Parse(Resources.Load<TextAsset>($"Datas/Config/{file}").text));

        StartDialog(0); // 测试用，应该在外部调用，和.json文件名对应
    }

    void Update()
    {
        if (isAwaitingInput)
        {
            // show arrow
        }
    }

    private void HandleInput()
    {
        if (isChoosing || !hasNext || isInterrupted) return; // 所有输入锁；日后需要做外部的键位映射，抽象成InputManager

        if (hasJump) // 跳转判定
        {
            hasJump = false;
            textAnimatorPlayer.SkipTypewriter();
            StartCoroutine(LoadNextLineCoolDown());
            PrepareDialog(jumpBranchLineIndex);
            return;
        }

        if (isAwaitingInput && !isCoolingDown)
        {
            isAwaitingInput = false;

            LoadNextLine();
        }
        else if (!isAwaitingInput)
        {
            textAnimatorPlayer.SkipTypewriter();
        }
    }

    void OnEvent(string message)
    {
        switch (message)
        {
            case "waitInput":
                // Auto load next line
                if (hasNext && autoPlay && !isInterrupted) StartCoroutine(WaitAutoPlay());

                isAwaitingInput = true;
                StartCoroutine(LoadNextLineCoolDown());
                break;
            case "skipInput":
                LoadNextLine();
                break;
        }

        if (message.StartsWith("ShowChoice"))
        {
            ParseChoiceMessage(message, out List<string> choices, out List<string> branchs);
            Messenger.Broadcast(MsgType.ShowChoices, choices);
            choiceBranchs = branchs;
            isChoosing = true;
            isAwaitingInput = false; // 选择时不需要等待输入
            isCoolingDown = false;
        }
        else if (message.StartsWith("JumpTo"))
        {
            string branchName = message.Substring(message.IndexOf("[[") + 2, message.IndexOf("]]") - message.IndexOf("[[") - 2);
            int branchLineIndex = dialogTables.TbdialogBranchMap.Get(branchName).StartRow;

            if (!isAwaitingInput)
            {
                hasJump = true; // 标记为跳转
                jumpBranchLineIndex = branchLineIndex; // 记录跳转的行索引
            }
            else
            {
                PrepareDialog(branchLineIndex);
            }
        }
    }

    private IEnumerator WaitAutoPlay()
    {
        yield return new WaitForSeconds(autoPlayWaitTime);
        LoadNextLine();
    }

    private IEnumerator LoadNextLineCoolDown()
    {
        isCoolingDown = true;
        yield return new WaitForSeconds(coolDownTime);
        isCoolingDown = false;
    }

    private void HandleChoice(int choiceIndex)
    {
        isChoosing = false;
        string branchName = choiceBranchs[choiceIndex];
        int branchLineIndex = dialogTables.TbdialogBranchMap.Get(branchName).StartRow;

        PrepareDialog(branchLineIndex);
    }

    public void PrepareDialog(int storyStartIndex) // 从其他地方调用时用这个,避免了第一句被跳过的问题
    {
        dialogLine = dialogTables.Tbdialog.Get(storyStartIndex);
        currentLineIndex = storyStartIndex;
        isAwaitingInput = false;
        SetDialog();
    }


    public void StartDialog(int storyStartIndex)
    {
        // Initialization
        currentLineIndex = storyStartIndex;
        hasNext = true;
        dialogLine = dialogTables.Tbdialog.Get(currentLineIndex); // 类型由表格决定

        // 以后可能会有多语言，所以要根据语言选择（比如有个语言选择界面，有个语言选择的全局变量，然后根据这个变量选择对应的语言）
        // 然后也会需要刷新对话框（读取当前行但是不同语言，而且文本动画器动画不会再播一次）
        LoadNextLine();
    }

    private void LoadNextLine()
    {
        dialogLine = dialogTables.Tbdialog.Get(currentLineIndex);

        // if (dialogLine.TextCn == "/end")
        // {
        //     // hasNext = false;
        //     // return;
        //     Messenger.Broadcast(MsgType.ChapterEnd); // 目前直接跳转到下一个章节
        // }
        // else
        // {
        //     SetDialog();
        // }

        SetDialog();
    }

    private void SetDialog()
    {
        characterName = dialogLine.NameCn;
        dialogText = dialogLine.TextCn;

        if (dialogLine.TextCn == "/end") { Messenger.Broadcast(MsgType.ChapterEnd); dialogText = "-章节完-"; return; }


        // 如果不是旁白，就设置立绘 （旁白暂未处理）
        string illustDir; // 目前是根据NameCn来映射的，暂时没必要改
        // 如果是KP就强行换成KP （暂时解决方案）
        if (characterName == "" || characterName == "以") { illustDir = GetIllustDirectory("KP"); }
        else { illustDir = GetIllustDirectory(characterName); }

        string nameColorStr = dialogTables.TbdialogIllustrationMap.Get(characterName).NameColor;
        Color nameColor = new Color();
        ColorUtility.TryParseHtmlString(nameColorStr, out nameColor);

        string illustFace = dialogLine.Illustration;
        if (illustFace == "") { illustFace = "Default"; } //什么也不填默认为Default表情
        illustration = Resources.Load<Sprite>(illustDir + illustFace); //路径为Map里设置的路径 + 表情差分（拼在一起需要和文件名完全一致）

        string direction = dialogLine.Direction;

        dialogUIPanel.SetDialog(illustration, characterName, dialogText, direction);
        dialogUIPanel.SetNameColor(nameColor);

        Messenger.Broadcast(MsgType.AddTextToLog, characterName, dialogText); // 记录到Log

        currentLineIndex++;
    }

    private string GetIllustDirectory(string name)
    {
        // 目前是根据NameCn来映射的，暂时没必要改
        string illustname = dialogTables.TbdialogIllustrationMap.Get(name).Directory;
        string prefix = dialogTables.TbdialogIllustrationMap.Get("General").Directory;
        string illustDir = $"{prefix}{illustname}/{illustname}_";
        return illustDir;
    }

    private void ParseChoiceMessage(string message, out List<string> options, out List<string> storyIds)
    {
        options = new List<string>();
        storyIds = new List<string>();

        int start = message.IndexOf(':');
        if (start == -1) return;

        string content = message.Substring(start + 1); // 截去 "ShowChoice:"
        var pairs = content.Split(';');

        foreach (var pair in pairs)
        {
            var arrowSplit = pair.Split(new string[] { "-" }, StringSplitOptions.None);
            if (arrowSplit.Length != 2) continue;

            string option = arrowSplit[0].Trim();
            string rightPart = arrowSplit[1];

            int left = rightPart.IndexOf("[[");
            int right = rightPart.IndexOf("]]");

            if (left != -1 && right != -1 && right > left)
            {
                string id = rightPart.Substring(left + 2, right - left - 2);
                options.Add(option);
                storyIds.Add(id);
            }
        }
    }

}