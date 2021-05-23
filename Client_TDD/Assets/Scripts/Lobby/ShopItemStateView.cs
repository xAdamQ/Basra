using System;
using UnityEngine;
using UnityEngine.UI;

public enum ShopItemState
{
    Locked,
    Unlocked,
    Set
}

public class ShopItemStateView : MonoBehaviour
{
    public Image Background;
    public Text Desc;
}