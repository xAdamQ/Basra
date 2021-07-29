using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RTLTMPro;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

[Title("emoji and text names are their ids")]
public class ChatSystem : MonoBehaviour
{
    public GameObject ChatPanel;

    [SerializeField] private Transform
        emojiParent,
        textParent;

    [SerializeField] private GameObject
        emojiMessageViewPrefab,
        textMessageViewPrefab;

    [Title("I use pos, rot and scale only")] [SerializeField]
    private Transform[] messagesStartTransform;
    [Title("I use pos only")] [SerializeField]
    private Transform[] messagesEndTransform;

    private Dictionary<string, string> texts = new Dictionary<string, string>();
    private Dictionary<string, Sprite> emojis = new Dictionary<string, Sprite>();

    public static ChatSystem I;

    /// <summary>
    /// uses RoomCanvas
    /// </summary>
    public static async UniTask Create()
    {
        I = (await Addressables.InstantiateAsync("chatSystem", RoomReferences.I.Canvas)).GetComponent<ChatSystem>();
    }

    private void Awake()
    {
        Controller.I.AssignRpc<int, string>(ShowMessage, nameof(RoomController));

        foreach (Transform emoji in emojiParent)
            emojis.Add(emoji.name, emoji.GetComponent<Image>().sprite);

        foreach (Transform text in textParent)
        {
            text.GetComponent<Translatable>().Awake(); //sets the default lang so I can use it here
            texts.Add(text.name, text.GetComponent<RTLTextMeshPro>().OriginalText);
        }
    }

    /// <summary>
    /// uses RoomSettings: my turn
    /// </summary>
    public void ShowMessage(int senderTurn, string messageId)
    {
        var senderPlace = PlayerBase.ConvertTurnToPlace(senderTurn, RoomSettings.I.MyTurn);

        GameObject obj;
        if (texts.ContainsKey(messageId))
        {
            obj = Instantiate(textMessageViewPrefab, transform);
            obj.transform.GetChild(0).GetComponent<RTLTextMeshPro>().text = texts[messageId];
        }
        else
        {
            obj = Instantiate(emojiMessageViewPrefab, transform);
            obj.transform.GetChild(0).GetComponent<Image>().sprite = emojis[messageId];
        }


        obj.transform.position = messagesStartTransform[senderPlace].position;
        obj.transform.rotation = messagesStartTransform[senderPlace].rotation;
        obj.transform.localScale = messagesStartTransform[senderPlace].localScale;

        obj.transform.DOScale(1, .3f);
        obj.transform.DOMove(messagesEndTransform[senderPlace].position, .4f);

        Destroy(obj, 2);
    }
}