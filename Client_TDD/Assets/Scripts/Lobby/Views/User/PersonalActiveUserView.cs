using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class PersonalActiveUserView : MinUserView
{
    private IRepository _repository;
    private PersonalFullUserView _personalFullUserView;
    [Inject]
    public void Construct(IRepository repository, PersonalFullUserView personalFullUserView)
    {
        _repository = repository;
        _personalFullUserView = personalFullUserView;
    }

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

    public void ShowFullView()
    {
        _personalFullUserView.Show();
    }

    protected virtual void OnInfoChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(MoneyAimTimeLeft):
                MoneyAimTimeLeft = _repository.PersonalFullInfo.MoneyAimTimeLeft;
                break;
            case nameof(Level):
                Level = _repository.PersonalFullInfo.Level;
                break;
            case nameof(Title):
                Title = _repository.PersonalFullInfo.Title;
                break;
            case nameof(Money):
                Money = _repository.PersonalFullInfo.Money;
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

    [SerializeField] private Text money;
    [SerializeField] private Text moneyAimTimeLeft;
}