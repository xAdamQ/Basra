using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Zenject;

public class FinalMuv : MonoBehaviour
{
    private string Id;

    [SerializeField] private Image picture;
    [SerializeField] private TMP_Text
        eatenCardsText,
        basrasText,
        bigBasrasText,
        winMoneyText;

    public void Init(MinUserInfo minUserInfo, UserRoomStatus oppoRoomResult)
    {
        Id = minUserInfo.Id;

        eatenCardsText.text = oppoRoomResult.EatenCards.ToString();
        basrasText.text = oppoRoomResult.Basras.ToString();
        bigBasrasText.text = oppoRoomResult.BigBasras.ToString();
        winMoneyText.text = oppoRoomResult.WinMoney.ToString();

        if (minUserInfo.IsPictureLoaded)
            SetPicture(minUserInfo.Picture);
        else
            minUserInfo.PictureLoaded += pic => SetPicture(pic);
    }

    public static async UniTaskVoid Instantiate(MinUserInfo minUserInfo, UserRoomStatus oppoRoomResult, Transform parent)
    {
        var asset = await Addressables.InstantiateAsync("finalMuv", parent);
        asset.GetComponent<FinalMuv>().Init(minUserInfo, oppoRoomResult);
    }

    private void SetPicture(Texture2D texture2D)
    {
        if (texture2D != null)
            picture.sprite =
                Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(.5f, .5f));
    }

    /// <summary>
    /// uses BlockingOperationManager, Controller, FullUserView
    /// </summary>
    public void ShowFullInfo()
    {
        BlockingOperationManager.I.Forget(Controller.I.GetPublicFullUserInfo(Id), info => FullUserView.I.Show(info));
    }

    public void AddFriend()
    {
        throw new System.NotImplementedException();
    }
}