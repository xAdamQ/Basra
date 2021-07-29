using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ChatSystem : MonoBehaviour
{
    [SerializeField] private Transform emojiParent, textParent;

    [SerializeField] private GameObject[] emojiMessages, textMessages;

    public Dictionary<string, string> Texts;
    public Dictionary<string, Sprite> Emojis;

    public static ChatSystem I;

    public static async UniTask Create()
    {
        I = (await Addressables.InstantiateAsync("chatPanel")).GetComponent<ChatSystem>();
    }

    private void Start()
    {
        foreach (Transform emoji in emojiParent)
            Emojis.Add(emoji.GetComponent<RoomMessage>().Id, emoji.GetComponent<Image>().sprite);

        foreach (Transform text in textParent)
            Texts.Add(text.GetComponent<RoomMessage>().Id, text.GetComponent<TMP_Text>().text);
    }
}