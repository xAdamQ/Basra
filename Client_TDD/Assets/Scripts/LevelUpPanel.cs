using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LevelUpPanel : MonoModule<LevelUpPanel>
{
    [SerializeField] private TMP_Text levelText, moneyRewardText;

    public static async UniTaskVoid Create(int newLevel, int moneyReward)
    {
        await Create("levelUpPanel", ProjectReferences.I.Canvas);

        I.levelText.text = newLevel.ToString();
        I.moneyRewardText.text = moneyReward.ToString();
    }


    public void Destroy() => Destroy(gameObject);
}