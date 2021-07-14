using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using Zenject;

public class PersonalActiveUserView : MinUserView
{
    [Inject] private IRepository _repository;

    private void Start()
    {
        _repository.PersonalFullInfo.PropertyChanged += OnInfoChanged;
        Init(_repository.PersonalFullInfo);
    }

    private void Init(PersonalFullUserInfo personalFullUserInfo)
    {
        base.Init(personalFullUserInfo);
        Money = personalFullUserInfo.Money;
        MoneyAimTimeLeft = personalFullUserInfo.MoneyAimTimeLeft;
    }

    public override void ShowFullInfo()
    {
        _fullUserView.Show(_repository.PersonalFullInfo);
    }

    protected virtual void OnInfoChanged(object sender, PropertyChangedEventArgs e)
    {
        var info = _repository.PersonalFullInfo;
        switch (e.PropertyName)
        {
            case nameof(info.MoneyAimTimeLeft):
                MoneyAimTimeLeft = info.MoneyAimTimeLeft;
                break;
            case nameof(info.Level):
                Level = info.Level;
                break;
            case nameof(info.SelectedTitleId):
                Title = Repository.Titles[info.SelectedTitleId];
                break;
            case nameof(info.Money):
                Money = info.Money;
                break;
        }
    }

    public int Money
    {
        set => money.text = value.ToString();
    }

    private TimeSpan? MoneyAimTimeLeft
    {
        set
        {
            if (value == null)
            {
                moneyAimTimeLeft.gameObject.SetActive(false);
            }
            else
            {
                moneyAimTimeLeft.text = $"60$ in {value:mm\\:ss}";
            }
        }
    }

    [SerializeField]
    private TMP_Text
        money,
        moneyAimTimeLeft;

    public void testwaitalot()
    {
        _blockingOperationManager.Forget(_controller.SendAsync("TestWaitAlot"));
        // _blockingOperationManager.Forget<MinUserInfo>(_controller.TestWaitWithReturn(), info => Debug.Log("info is" + info.Name));
    }
}