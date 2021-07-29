using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    public static Background I;
    /// <summary>
    /// because of the parent, it's destroyed with module group
    /// </summary>
    public static async UniTask Create()
    {
        I = (await Addressables.InstantiateAsync("background")).GetComponent<Background>();

        I.GetComponent<Canvas>().worldCamera = Camera.main;
        I.transform.SetSiblingIndex(0);

        I.SetForLobby();
    }

    /// <summary>
    /// uses RoomController
    /// </summary>
    public void SetForRoom(List<FullUserInfo> userInfos)
    {
        var maxLevel = userInfos.Max(u => u.Xp);
        var bgId = userInfos.First(u => u.Xp == maxLevel).SelectedBackground;


        // var sprietHandle =
        // Addressables.LoadAssetAsync<Sprite>($"bg{bgIndex}");
        // var sprite = await sprietHandle;
        //todo bgs with be collected into one sheet anyway

        // GetComponent<Image>().sprite = sprite;

        // Addressables.Release(sprietHandle);

        Extensions.LoadAndReleaseAsset<Sprite>(((BackgroundType) bgId).ToString(), sprite => GetComponent<Image>().sprite = sprite)
            .Forget(e => throw e);

        RoomController.I.Destroyed += I.SetForLobby;
    }


    public void SetForLobby()
    {
        Extensions.LoadAndReleaseAsset<Sprite>(BackgroundType.brownLeaf.ToString(), 
                sprite => GetComponent<Image>().sprite = sprite)
            .Forget(e => throw e);
    }
}