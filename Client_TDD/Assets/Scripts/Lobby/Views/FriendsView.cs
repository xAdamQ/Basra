using UnityEngine;
using Zenject;

public class FriendsView : MonoBehaviour
{
    private PublicMinUserView.BasicFactory _minUserViewFactory;
    private IRepository _repository;

    /// <summary>
    /// this is legal because they are the same unit
    /// </summary>
    [SerializeField] private Transform container;

    [Inject]
    public void Construct(PublicMinUserView.BasicFactory minUserViewFactory, IRepository repository)
    {
        _minUserViewFactory = minUserViewFactory;
        _repository = repository;
    }

    private void Start()
    {
        for (int i = 0; i < _repository.TopFriends.Length; i++)
        {
            var view = _minUserViewFactory.Create();
            view.transform.SetParent(container);
            view.transform.localScale = Vector3.one;
            view.Init(_repository.TopFriends[i]);
        }
    }
}