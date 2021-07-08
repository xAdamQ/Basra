using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System.Linq;

public interface IRoomController
{
    void StartRoomRpc(List<int> handCardIds, List<int> groundCardIds); //trivial to test
    void DestroyModuleGroup();
    void FinalizeRoom(FinalizeResult finalizeResult);
}


public class RoomController : IRoomController, IInitializable, System.IDisposable
{
    [Inject] private readonly IRepository _repository;
    [Inject] private readonly RoomUserView.IManager _ruvManager;
    [Inject] private readonly IController _controller;
    [Inject] private readonly RoomSettings _roomSettings;
    [Inject] private readonly IBlockingPanel _blockingPanel;
    [Inject] private readonly ReferenceInstantiator<RoomInstaller> _referenceInstantiator;

    [Inject] private readonly RoomInstaller.References _roomRefs;

    [Inject] private readonly ICoreGameplay _coreGameplay;

    [InjectOptional] private readonly ActiveRoomState _activeRoomState;

    //todo this should init first
    public void Initialize()
    {
        _ruvManager.Init(_roomSettings.UserInfos, _roomSettings.MyTurn);

        _coreGameplay.CreatePlayers();

        AssignRpcs();

        if (_activeRoomState == null)
            _controller.SendAsync("Ready").Forget(e => throw e);
        else
            LateStart().Forget(e => throw e);
    }

    private async UniTask LateStart()
    {
        await UniTask.DelayFrame(2);

        _coreGameplay.ResumeGame(_activeRoomState.MyHand, _activeRoomState.Ground, _activeRoomState.HandCounts,
            _activeRoomState.CurrentTurn);
    }

    public void Dispose()
    {
        _controller.RemoveModuleRpcs(nameof(RoomController));
    }

    private void AssignRpcs()
    {
        var moduleGroupName = nameof(RoomController);

        _controller.AssignRpc<List<int>, List<int>>(StartRoomRpc, moduleGroupName);
        _controller.AssignRpc<FinalizeResult>(FinalizeRoom, moduleGroupName);
    }

    public void FinalizeRoom(FinalizeResult finalizeResult)
    {
        _coreGameplay.EatLast(finalizeResult.LastEaterTurnId);

        RoomResultPanel.Instantiate(_referenceInstantiator, _roomRefs, finalizeResult.RoomXpReport,
            _repository.PersonalFullInfo, finalizeResult.PersonalFullUserInfo, _roomSettings.BetChoice);

        _repository.PersonalFullInfo = finalizeResult.PersonalFullUserInfo;
    }

    public void StartRoomRpc(List<int> handCardIds, List<int> groundCardIds)
    {
        _coreGameplay.BeginGame(handCardIds, groundCardIds);
        _blockingPanel.Hide();
    }

    public void DestroyModuleGroup()
    {
        Object.Destroy(Object.FindObjectOfType<RoomInstaller>().gameObject);
    }

    
    
    public class Factory : PlaceholderFactory<RoomSettings, ActiveRoomState, RoomController>
    {
    }
}