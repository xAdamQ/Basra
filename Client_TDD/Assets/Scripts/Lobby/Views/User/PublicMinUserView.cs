using System;
using System.ComponentModel;
using Cysharp.Threading.Tasks;
using MiscUtil.Collections.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class PublicMinUserView : MonoBehaviour
{
    [SerializeField] Text displayName;
    [SerializeField] Image profilePicture;
    [SerializeField] Text level;
    [SerializeField] Text title;

    private IController _controller;
    private PublicFullUserView _publicFullUserView;

    [Inject]
    public void Construct(IController controller, PublicFullUserView publicFullUserView)
    {
        _controller = controller;
        _publicFullUserView = publicFullUserView;
    }

    public void Init(PublicMinUserInfo publicMinUserInfo)
    {
        id = publicMinUserInfo.Id;
        Level = publicMinUserInfo.Level;
        DisplayName = publicMinUserInfo.DisplayName;
        Picture = publicMinUserInfo.Picture;
        Title = publicMinUserInfo.Title;
    }

    public void RequestFullData()
    {
        _controller.GetPublicFullUserInfo(id).ContinueWith(info => _publicFullUserView.Show(info));
        //ask the server for it for all users except in personal
        //this request public, the other request personal
    }

    private string id;
    public int Level
    {
        set => level.text = value.ToString();
    }
    public string DisplayName
    {
        set => displayName.text = value;
    }
    public Texture2D Picture
    {
        set => profilePicture.sprite =
            Sprite.Create(value, new Rect(0, 0, value.width, value.height), new Vector2(.5f, .5f));
    }
    public string Title
    {
        set => title.text = value;
    }

    public class BasicFactory : PlaceholderFactory<PublicMinUserView>
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