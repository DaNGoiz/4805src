using UnityEngine;
using cfg;
using SimpleJSON;

public class CharacterInGame : MonoBehaviour
{
    public int id;
    public bool isWalking;
    private Sprite characterSprite;

    void Start()
    {
        characterSprite = GetComponent<SpriteRenderer>().sprite;

        // 由生成器注入贴图和id

    }

    void Update()
    {
        // if (isWalking)
        // {
        //     transform.Translate(Vector3.right * Time.deltaTime);
        //     if (transform.position.x > 5 || transform.position.x < -5)
        //     {
        //         transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
        //     }
        // }
    }

    void OnMouseDown()
    {
        Messenger.Broadcast<int>("OnWorldCharacterClicked", id);
    }
}
