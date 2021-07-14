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
    public PersonalFullUserInfo()
    {

        //todo test this
        if (MoneyAimTimeLeft != null)
            UniTask.Create(async () =>
            {
                await UniTask.DelayFrame(1);
                DecreaseMoneyAimTimeLeft().Forget();
            });

    }

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
    /// I use this rather than timestamp when the request is done
    /// because datetime.now is not universal for all clients
    /// even if I sent current server time and made MoneyAimTimeLeft = server time - request time
    /// it would be the same as sending it directly form the server  
    /// </summary>
    public TimeSpan? MoneyAimTimeLeft
    {
        get => moneyAimTimeLeft;
        set
        {
            moneyAimTimeLeft = value;
            NotifyPropertyChanged();
        }
    }
    private TimeSpan? moneyAimTimeLeft;

    // public override string Title
    // {
    //     get => _title;
    //     set
    //     {
    //         _title = value;
    //         NotifyPropertyChanged();
    //     }
    // }
    // private string _title;

    public override int Level
    {
        get => level;
        set
        {
            level = value;
            NotifyPropertyChanged();
        }
    }
    private int level;

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
        var updateRate = 1;
        while (MoneyAimTimeLeft > TimeSpan.Zero)
        {
            Debug.Log("info is changing");
            MoneyAimTimeLeft = MoneyAimTimeLeft.Value.Subtract(TimeSpan.FromSeconds(updateRate));
            await UniTask.Delay(TimeSpan.FromSeconds(updateRate));
        }

        MoneyAimTimeLeft = null;
    }
}