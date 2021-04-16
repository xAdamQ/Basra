using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// this is active by default, we could be more specific and make active user info type for
/// class that has INotifyPropertyChanged
/// </summary>
public class PersonalFullUserInfo : PublicFullUserInfo, INotifyPropertyChanged
{
    public int Money
    {
        get => _money;
        set
        {
            _money = value;
            NotifyPropertyChanged();
        }
    }
    private int _money;

    public TimeSpan MoneyAimTimeLeft
    {
        get => _moneyAimTimeLeft;
        set
        {
            _moneyAimTimeLeft = value;
            NotifyPropertyChanged();
        }
    }
    private TimeSpan _moneyAimTimeLeft;

    public override string Title
    {
        get => _title;
        set
        {
            _title = value;
            NotifyPropertyChanged();
        }
    }
    private string _title;

    public override int Level
    {
        get => _level;
        set
        {
            _level = value;
            NotifyPropertyChanged();
        }
    }
    private int _level;

    public int FlipWinCount { get; set; }
    public object ActiveRoomData { get; set; }
    public List<string> Titles { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async UniTask DecreaseMoneyAimTimeLeft()
    {
        var updateRate = 3;
        while (MoneyAimTimeLeft != null && MoneyAimTimeLeft > TimeSpan.Zero)
        {
            Debug.Log("info is changing");
            MoneyAimTimeLeft = MoneyAimTimeLeft.Subtract(TimeSpan.FromSeconds(updateRate));
            await UniTask.Delay(TimeSpan.FromSeconds(updateRate));
        }
    }
}