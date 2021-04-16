using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalRCore;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public interface IController
{
    void SetGameInfo(PersonalFullUserInfo myFullUserInfo, PublicMinUserInfo[] yesterdayChampions,
        PublicMinUserInfo[] topFriends);

    void ConnectToServer(string token);
    UniTask<PublicFullUserInfo> GetPublicFullUserInfo(string userId);
    UniTask<object> SendAsync(string method, params object[] args);
}

public class Controller : IController, IInitializable
{
    private readonly ZenjectSceneLoader _zenjectSceneLoader;
    private readonly IRepository _repository;
    private readonly HubConnection HubConnection;

    [Inject]
    public Controller(ZenjectSceneLoader zenjectSceneLoader, IRepository repository)
    {
        _zenjectSceneLoader = zenjectSceneLoader;
        _repository = repository;
    }

    public void Initialize()
    {
        throw new NotImplementedException();
    }

    public void SetGameInfo(PersonalFullUserInfo myFullUserInfo, PublicMinUserInfo[] yesterdayChampions,
        PublicMinUserInfo[] topFriends)
    {
        _repository.PersonalFullInfo = myFullUserInfo;
        _repository.YesterdayChampions = yesterdayChampions;
        _repository.TopFriends = topFriends;
    }

    public void ConnectToServer(string token)
    {
    }

    public async UniTask<PublicFullUserInfo> GetPublicFullUserInfo(string userId)
    {
        throw new NotImplementedException();
    }

    public async UniTask ThrowCard(int cardId)
    {
        await HubConnection.SendAsync("Throw", cardId);
    }

    public async UniTask<object> SendAsync(string method, params object[] args)
    {
        return await HubConnection.SendAsync(method, args);
    }

    private void OnConnected()
    {
        Debug.Log("connected to the server successfully");
    }
}