using System;
using System.ComponentModel;
using UnityEngine;
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
        _personalFullUserView.Show(_repository.PersonalFullInfo);
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

    [SerializeField] private Text money;
    [SerializeField] private Text moneyAimTimeLeft;
}