using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using SimpleJSON;

public class ClueInGame : MonoBehaviour
{
    // 如果附和当前进度，就显示
    // 可以点击，会被收入线索板
    public int clueID;
    private bool isAvailable = true; // 暂时
    private bool isCollected = false;

    private cfg.intelligence.clue_map clueData;
    private cfg.intelligence.Tbclue_map clueMap;

    void Start()
    {
        // 更新整体stage的时候进行messenger广播
        // 读取clue的贴图
        clueMap = new cfg.intelligence.Tbclue_map(
            JSON.Parse(Resources.Load<TextAsset>("Datas/Config/intelligence_tbclue_map").text)
        );
        clueData = clueMap.Get(clueID);
        SetSprite(Resources.Load<Sprite>("Textures/Clue/" + clueData.ImagePath));
    }

    void StageCheck()
    {
        // 每次更新整体stage的时候就检查当前进度
    }

    // raycast检测点击
    void OnMouseDown()
    {
        if (isAvailable && !isCollected)
        {
            isCollected = true;

            Messenger.Broadcast("OnWorldClueClicked", clueID);

            gameObject.SetActive(false);
        }
    }

    private void SetSprite(Sprite newSprite)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Vector2 oldSize = sr.sprite.bounds.size;

        sr.sprite = newSprite;

        Vector2 newSize = sr.sprite.bounds.size;
        if (newSize != Vector2.zero)
        {
            Vector3 scale = transform.localScale;
            scale.x *= oldSize.x / newSize.x;
            scale.y *= oldSize.y / newSize.y;
            transform.localScale = scale;
        }
    }
}