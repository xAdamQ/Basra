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
    public static async UniTaskVoid Create()
    {
        I = (await Addressables.InstantiateAsync("background")).GetComponent<Background>();

        I.GetComponent<Canvas>().worldCamera = Camera.main;
        I.transform.SetSiblingIndex(0);

        I.SetForLobby().Forget();
    }

    /// <summary>
    /// uses RoomController
    /// </summary>
    public async UniTaskVoid SetForRoom(List<FullUserInfo> userInfos)
    {
        var maxLevel = userInfos.Max(u => u.Level);
        var bgIndex = userInfos.First(u => u.Level == maxLevel).SelectedBackground;

        var sprite = await Addressables.LoadAssetAsync<Sprite>($"bg{bgIndex}");
        //todo bgs with be collected into one sheet anyway

        GetComponent<Image>().sprite = sprite;

        RoomController.I.Destroyed += UniTask.Action(() => I.SetForLobby());
    }

    public async UniTaskVoid SetForLobby()
    {
        var sprite = await Addressables.LoadAssetAsync<Sprite>($"bg{0}");
        GetComponent<Image>().sprite = sprite;
    }
}