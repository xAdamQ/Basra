using System;
using System.ComponentModel;
using Cysharp.Threading.Tasks;
using MiscUtil.Collections.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Image))]
public class MinUserView : MonoBehaviour
{
    [SerializeField] private Text displayName;
    [SerializeField] private Image profilePicture;
    [SerializeField] private Text level;
    [SerializeField] private Text title;

    protected IController _controller;
    protected FullUserView _fullUserView;

    [Inject]
    public void Construct(IController controller, FullUserView fullUserView)
    {
        _controller = controller;
        _fullUserView = fullUserView;
    }

    public void Init(MinUserInfo minUserInfo)
    {
        id = minUserInfo.Id;
        Level = minUserInfo.Level;
        DisplayName = minUserInfo.Name;
        Title = Repository.Titles[minUserInfo.SelectedTitleId];

        //upgrade current types by inheritance 
        //

        if (minUserInfo.IsPictureLoaded)
            SetPicture(minUserInfo.Picture);
        else
            minUserInfo.PictureLoaded += pic => SetPicture(pic);
    }


    /// <summary>
    /// personal view overrides this 
    /// </summary>
    public virtual void ShowFullInfo()
    {
        //todo this forget thing maybe wrong
        _controller.GetPublicFullUserInfo(id).ContinueWith(info => _fullUserView.Show(info)).Forget();
    }

    private void SetPicture(Texture2D texture2D)
    {
        if (texture2D != null)
            profilePicture.sprite =
                Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(.5f, .5f));
    }

    protected string id;
    public int Level
    {
        set => level.text = value.ToString();
    }
    public string DisplayName
    {
        set => displayName.text = value;
    }

    public string Title
    {
        set => title.text = value;
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