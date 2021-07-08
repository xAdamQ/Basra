using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Image))]
public class MinUserView : MonoBehaviour
{
    [SerializeField] private Text displayName;
    [SerializeField] private Text level;
    [SerializeField] private Text title;
    [SerializeField] private Image picture;

    [Inject] protected BlockingOperationManager _blockingOperationManager;
    [Inject] protected IController _controller;
    [Inject] protected FullUserView _fullUserView;


    public void Init(MinUserInfo minUserInfo)
    {
        Id = minUserInfo.Id;
        Level = minUserInfo.Level;
        DisplayName = minUserInfo.Name;
        Title = Repository.Titles[minUserInfo.SelectedTitleId];

        if (minUserInfo.IsPictureLoaded)
            SetPicture(minUserInfo.Picture);
        else
            minUserInfo.PictureLoaded += pic => SetPicture(pic);
    }

    private void SetPicture(Texture2D texture2D)
    {
        if (texture2D != null)
            picture.sprite =
                Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(.5f, .5f));
    }
    /// <summary>
    /// personal view overrides this 
    /// </summary>
    public virtual void ShowFullInfo()
    {
        _blockingOperationManager.Forget(_controller.GetPublicFullUserInfo(Id), info => _fullUserView.Show(info));
    }

    protected string Id;

    public int Level
    {
        set
        {
            if (level)
                level.text = value.ToString();
        }
    }
    public string DisplayName
    {
        set
        {
            if (displayName)
                displayName.text = value;
        }
    }
    public string Title
    {
        set
        {
            if (title)
                title.text = value;
        }
    }

    public class BasicFactory : PlaceholderFactory<MinUserView>
    {
    }

    // public class Factory : IFactory<Transform, MinUserInfo, MinUserView>
    // {
    //     private readonly BasicFactory _basicFactory;
    //     private readonly IInstantiator _instantiator;
    //     private readonly GameObject _prefab;
    //
    //     public Factory(BasicFactory basicFactory, IInstantiator instantiator,
    //         GameObject prefab /*argument set by installer, not a service*/)
    //     {
    //         _basicFactory = basicFactory;
    //         _instantiator = instantiator;
    //         _prefab = prefab;
    //     }
    //
    //     public MinUserView Create(Transform parent, MinUserInfo info)
    //     {
    //         var view = _basicFactory.Create()
    //         // var view = _instantiator.InstantiatePrefab(_prefab, parent).GetComponent<MinUserView>();
    //         view.Init(info);
    //         return view;
    //     }
    // }
}