using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LevelUpPanel : MonoModule<LevelUpPanel>
{
    [SerializeField] private TMP_Text levelText, moneyRewardText;

    [Rpc("LevelUp")]
    //money is added to the whole personal info object is updated on finalize 
    public static async UniTaskVoid Create(int newLevel, int moneyReward)
    {
        await Create("levelUpPanel", ProjectReferences.I.Canvas);

        I.levelText.text = newLevel.ToString();
        I.moneyRewardText.text = moneyReward.ToString();
    }
}