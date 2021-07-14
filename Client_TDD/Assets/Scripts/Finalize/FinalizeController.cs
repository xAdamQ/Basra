using Zenject;

public class FinalizeController : IInitializable
{
    //args
    [Inject] readonly FinalizeResult _finalizeResult;
    [Inject] readonly RoomSettings _roomSettings;

    //proj
    [Inject] private readonly ReferenceInstantiator<FinalizeInstaller> _referenceInstantiator;

    //fin
    [Inject] private readonly FinalizeInstaller.References _finalizeRefs;


    public void Initialize()
    {
        for (int i = 0; i < _roomSettings.UserInfos.Count; i++)
        {
            if (i == _roomSettings.MyTurn) continue;

            FinalMuv.Consrtuct(_roomSettings.UserInfos[i], _finalizeResult.UserRoomStatus[i], _finalizeRefs.Canvas).Forget();
        }

        RoomResultPanel.Instantiate(_referenceInstantiator, _finalizeRefs,
            _finalizeResult.RoomXpReport, _finalizeResult.PersonalFullUserInfo,
            _finalizeResult.UserRoomStatus[_roomSettings.MyTurn]);
    }

    public class Factory : PlaceholderFactory<FinalizeResult, RoomSettings, FinalizeController>
    {
    }
}
