using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PrizeView : MonoModule<PrizeView>
{
    [SerializeField] private TMP_Text prizeText;

    public static void Create()
    {
        Create("prizeView", RoomReferences.I.Canvas)
            .Forget(e => throw e);
    }

    private void Start()
    {
        prizeText.text = RoomSettings.I.TotalPrize.ToString();
    }
}