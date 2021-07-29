using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// this is active by default, we could be more specific and make active user info type for
/// class that has INotifyPropertyChanged
/// </summary>
public class PersonalFullUserInfo : FullUserInfo, INotifyPropertyChanged
{
    public override int Money
    {
        get => money;
        set
        {
            money = value;
            NotifyPropertyChanged();
        }
    }
    private int money;

    /// <summary>
    /// it's guaranteed to be the valid value when less than ConstData.MoneyAimTime
    /// but when it's bigger it will just bigger or equal but the value won't be valid
    /// 
    /// because datetime.now is not universal for all clients
    /// and I don't know if utc now is universal or not!
    /// </summary>
    public double? MoneyAimTimePassed
    {
        get => _moneyAimTimePassed;
        set
        {
            _moneyAimTimePassed = value;
            NotifyPropertyChanged();
        }
    }
    private double? _moneyAimTimePassed;

    public int MoneyAidRequested { get; set; }

    public override int Xp
    {
        get => xp;
        set
        {
            xp = value;
            NotifyPropertyChanged();
        }
    }
    private int xp;

    public int FlipWinCount { get; set; }

    public List<int> Titles { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    [NotifyPropertyChangedInvocator]
    protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public async UniTaskVoid DecreaseMoneyAimTimeLeft()
    {
        var updateRateInSeconds = 1;
        while (MoneyAimTimePassed < ConstData.MoneyAimTime)
        {
            Debug.Log("info is changing");

            // MoneyAimTimePassed = MoneyAimTimePassed.Value.Add(TimeSpan.FromSeconds(updateRateInSeconds));
            MoneyAimTimePassed += updateRateInSeconds;

            await UniTask.Delay(TimeSpan.FromSeconds(updateRateInSeconds));
        }
    }
}