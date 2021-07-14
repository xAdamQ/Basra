using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Zenject;

public class FinalMuv : MonoBehaviour
{
    protected string Id;

    [SerializeField] private UserRoomStatusView oppoRoomResultView;
    [SerializeField] private Image picture;

    [Inject] protected BlockingOperationManager _blockingOperationManager;
    [Inject] protected IController _controller;
    [Inject] protected FullUserView _fullUserView;

    public void Init(MinUserInfo minUserInfo, UserRoomStatus OppoRoomResult)
    {
        Id = minUserInfo.Id;

        //oppoRoomResultView.Init(OppoRoomResult);

        if (minUserInfo.IsPictureLoaded)
            SetPicture(minUserInfo.Picture);
        else
            minUserInfo.PictureLoaded += pic => SetPicture(pic);
    }

    public static async UniTaskVoid Consrtuct(MinUserInfo minUserInfo, UserRoomStatus OppoRoomResult, Transform parent)
    {
        var asset = await Addressables.InstantiateAsync("finalMuv", parent);
        asset.GetComponent<FinalMuv>().Init(minUserInfo, OppoRoomResult);
    }

    private void SetPicture(Texture2D texture2D)
    {
        if (texture2D != null)
            picture.sprite =
                Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(.5f, .5f));
    }

    public void ShowFullInfo()
    {
        _blockingOperationManager.Forget(_controller.GetPublicFullUserInfo(Id), info => _fullUserView.Show(info));
    }

    public void AddFriend()
    {
        throw new System.NotImplementedException();
    }
}

////it's a vertical layout group huld muvs as children
//public class FinalMuvManager : MonoBehaviour
//{
//    [Inject] private readonly IInstantiator _instantiator;
//    [Inject] private readonly FinalMuv.BasicFactory basicFactory;


//}