using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// only single instance possible at a time
/// 
/// </summary>
public class PersonalFullUserView : FullUserView
{
    [SerializeField] private Text money;
    //todo  [SerializeField] private Text[] flipWin;

    private IRepository _repository;

    [Inject]
    public void Construct(IRepository repository)
    {
        _repository = repository;
    }

    public void Show()
    {
        base.Show(_repository.PersonalFullInfo);
        money.text = _repository.PersonalFullInfo.Money.ToString();
    }
}