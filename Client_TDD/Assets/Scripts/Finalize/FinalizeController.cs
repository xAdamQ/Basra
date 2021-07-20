using System;
using Cysharp.Threading.Tasks;
using Moq;
using UnityEngine;
using UnityEngine.AddressableAssets;

// public interface IFinalizeController
// {
// }

public class FinalizeController
    // : IFinalizeController, IModule
{
    // public Container Container => Container.Room;

    // //args
    // [Inject] readonly FinalizeResult _finalizeResult;
    // [Inject] readonly RoomSettings _roomSettings;

    //proj
    // [Inject] private readonly ReferenceInstantiator<FinalizeInstaller> _referenceInstantiator;

    //fin
    // [Inject] private readonly FinalizeInstaller.References _finalizeRefs;


    // public static IFinalizeController I;

    // public FinalizeController(RoomSettings roomSettings, FinalizeResult finalizeResult)
    // {
    // }

    public static async UniTask Construct(Transform moduleCanvas, RoomSettings roomSettings, FinalizeResult finalizeResult)
    {
        // if (I != null)
        // throw new Exception($"singleton {nameof(IFinalizeController)} already exists");

        var finalMuvParent = (await Addressables.InstantiateAsync("finalMuvParent", moduleCanvas)).transform;

        for (int i = 0; i < roomSettings.UserInfos.Count; i++)
        {
            if (i == roomSettings.MyTurn) continue;

            FinalMuv.Instantiate(roomSettings.UserInfos[i], finalizeResult.UserRoomStatus[i], finalMuvParent).Forget();
        }

        RoomResultPanel.Instantiate(moduleCanvas, finalizeResult.RoomXpReport, finalizeResult.PersonalFullUserInfo,
            finalizeResult.UserRoomStatus[roomSettings.MyTurn]).Forget();
    }


    // public void Initialize()
    // {
    //     for (int i = 0; i < _roomSettings.UserInfos.Count; i++)
    //     {
    //         if (i == _roomSettings.MyTurn) continue;
    //
    //         FinalMuv.Consrtuct(_roomSettings.UserInfos[i], _finalizeResult.UserRoomStatus[i], _finalizeRefs.Canvas).Forget();
    //     }
    //
    //     RoomResultPanel.Instantiate(_referenceInstantiator, _finalizeRefs,
    //         _finalizeResult.RoomXpReport, _finalizeResult.PersonalFullUserInfo,
    //         _finalizeResult.UserRoomStatus[_roomSettings.MyTurn]);
    // }
    //
    // public class Factory : PlaceholderFactory<FinalizeResult, RoomSettings, FinalizeController>
    // {
    // }
}