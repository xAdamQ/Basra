using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System.Linq;

public interface IRoomController
{
    void StartRoomRpc(List<int> handCardIds, List<int> groundCardIds); //trivial to test
    void DestroyModule();
    void FinalizeGame(FinalizeResult finalizeResult);
}

public enum PlayerType
{
    Me,
    Oppo
}

public class RoomController : IRoomController, IInitializable, System.IDisposable
{
    [Inject] private readonly IRepository _repository;
    [Inject] private readonly IGround _ground;
    [Inject] private readonly RoomUserView.Factory _roomUserViewFactory;
    [Inject] private readonly IController _controller;
    [Inject] private readonly RoomSettings _roomSettings;
    [Inject] private readonly IBlockingPanel _blockingPanel;
    [Inject] private readonly ReferenceInstantiator _referenceInstantiator;

    [Inject] private readonly RoomInstaller.Refernces _roomRefs;

    [Inject] private readonly ICoreGameplay _coreGameplay;
    // [Inject] private readonly IRoomUserViewManager _roomUserViewManager;

    //todo this should init first
    public void Initialize()
    {
        _coreGameplay.CreatePlayers();

        var playersInfo = new List<MinUserInfo>() { _repository.PersonalFullInfo };
        playersInfo.AddRange(_roomSettings.OpposInfo.Select(_ => _.FullUserInfo));
        _roomUserViewFactory.Create(playersInfo);

        AssignRpcs();

        _controller.SendAsync("Ready").Forget(e => throw e);
    }

    public void Dispose()
    {
        _controller.RemoveModuleRpcs(nameof(RoomController));
    }

    private void AssignRpcs()
    {
        _controller.AssignRpc<List<int>, List<int>>(StartRoomRpc, nameof(RoomController));
    }

    public void FinalizeGame(FinalizeResult finalizeResult)
    {
        _repository.PersonalFullInfo = finalizeResult.PersonalFullUserInfo;

        RoomResultPanel.Instantiate(_referenceInstantiator, _roomRefs, finalizeResult.RoomXpReport, _repository.PersonalFullInfo);
    }

    public void StartRoomRpc(List<int> handCardIds, List<int> groundCardIds)
    {
        _coreGameplay.BeginGame(handCardIds, groundCardIds);
        _blockingPanel.Hide();
    }

    public void DestroyModule()
    {
        Object.Destroy(Object.FindObjectOfType<LobbyInstaller>().gameObject);
    }

    public class Factory : PlaceholderFactory<RoomSettings, RoomController>
    {
    }
}