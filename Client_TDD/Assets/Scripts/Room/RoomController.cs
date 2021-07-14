using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

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
    [Inject] private readonly IBlockingPanel _blockingPanel;
    [Inject] private readonly ICoreGameplay _coreGameplay;
    [Inject] private readonly FinalizeController.Factory _finalizeFactory;

    //args
    [InjectOptional] private readonly ActiveRoomState _activeRoomState;
    [Inject] private readonly RoomSettings _roomSettings;

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
        UniTask.Create(async () =>
        {
            await _coreGameplay.EatLast(finalizeResult.LastEaterTurnId);

            DestroyModuleGroup();

            _finalizeFactory.Create(finalizeResult, _roomSettings);

            _repository.PersonalFullInfo = finalizeResult.PersonalFullUserInfo;
        });
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