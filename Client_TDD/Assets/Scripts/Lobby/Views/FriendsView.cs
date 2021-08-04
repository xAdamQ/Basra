using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class FriendsView : MonoBehaviour
{
    public static async UniTask Create()
    {
        await Addressables.InstantiateAsync("friendsView", LobbyReferences.I.Canvas);
    }

    /// <summary>
    /// this is legal because they are the same unit
    /// </summary>
    [SerializeField] private Transform container;

    private async UniTaskVoid Start()
    {
        for (int i = 0; i < Repository.I.TopFriends?.Length; i++)
        {
            await MinUserView.Create(Repository.I.TopFriends[i], container);
        }
    }
}