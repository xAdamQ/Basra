using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Basra.Common;
using UnityEngine;

public interface ICoreGameplay
{
    IPlayerBase PlayerInTurn { get; }
    int CurrentTurn { get; }

    UniTask CreatePlayers();
    void InitialTurn();
    void NextTurn(bool endPrevTurn = true);
    void BeginGame(List<int> myHand, List<int> groundCards);
    void Distribute(List<int> handCardIds);
    void LastDistribute(List<int> handCardIds);
    UniTask EatLast(int lastEaterTurnId);
    void ResumeGame(List<int> myHand, List<int> ground, List<int> handCounts, int currentTurn);
}

public class CoreGameplay : ICoreGameplay
{
    public static ICoreGameplay I;

    public CoreGameplay()
    {
        I = this;

        Initialize().Forget();
    }

    public async UniTaskVoid Initialize()
    {
        await UniTask.DelayFrame(3);

        RoomController.I.Destroyed += OnRoomDestroyed;
        AssignRpcs();
    }

    public void OnRoomDestroyed()
    {
        Controller.I.RemoveModuleRpcs(nameof(CoreGameplay));
    }

    private List<IPlayerBase> Players { get; } = new List<IPlayerBase>();
    public IPlayerBase PlayerInTurn => Players[CurrentTurn];

    //this is server turn
    public int CurrentTurn { get; private set; }

    public async UniTask EatLast(int lastEaterTurnId)
    {
        await Players[lastEaterTurnId].EatLast();
    }

    public async UniTask CreatePlayers()
    {
        var oppoPlaceCounter = 1;
        //oppo place starts at 1 to 3

        for (int i = 0; i < RoomSettings.I.Capacity; i++)
        {
            PlayerBase player = null;
            if (RoomSettings.I.MyTurn == i)
            {
                player = await PlayerBase.Create(RoomSettings.I.UserInfos[i].SelectedCardback, 0, i);
                Players.Add(player);
                MyPlayer = player as IPlayer;
            }
            else
            {
                player = await PlayerBase.Create(RoomSettings.I.UserInfos[i].SelectedCardback, oppoPlaceCounter++, i);
                Players.Add(player);
                Oppos.Add(player as IOppo);
            }
        }
    }

    private List<IOppo> Oppos { get; } = new List<IOppo>();
    private IPlayer MyPlayer { get; set; }

    public void InitialTurn()
    {
        CurrentTurn = -1;
        NextTurn(false);
    }

    public void NextTurn(bool endPrevTurn = true)
    {
        if (endPrevTurn) PlayerInTurn.EndTurn();

        CurrentTurn = ++CurrentTurn % RoomSettings.I.Capacity;

        if (PlayerInTurn.HandCards.Count == 0 && isLastDistribute) return;

        PlayerInTurn.StartTurn();
    }

    public void ResumeGame(List<int> myHand, List<int> ground, List<int> handCounts, int currentTurn)
    {
        Ground.I.Distribute(ground);

        MyPlayer.Distribute(myHand);

        for (int i = 0; i < handCounts.Count; i++)
        {
            if (i == RoomSettings.I.MyTurn) continue;

            ((IOppo)Players[i]).Distribute(handCounts[i]);
        }

        CurrentTurn = currentTurn - 1;
        NextTurn(false);
    }

    #region rpcs

    private void AssignRpcs()
    {
        Controller.I.AssignRpc<List<int>>(Distribute, nameof(CoreGameplay));
        Controller.I.AssignRpc<List<int>>(LastDistribute, nameof(CoreGameplay));
        Controller.I.AssignRpc<ThrowResult>(MyThrowResult, nameof(CoreGameplay));
        Controller.I.AssignRpc<ThrowResult>(ForcePlay, nameof(CoreGameplay));
        Controller.I.AssignRpc<ThrowResult>(CurrentOppoThrow, nameof(CoreGameplay));
    }

    private bool isLastDistribute;

    private void DistributeBase(List<int> handCardIds, bool last)
    {
        UniTask.Create(async () =>
        {
            // await UniTask.Delay(1500); //if there's a pending throw and eat

            await MyPlayer.Distribute(handCardIds);

            foreach (var oppo in Oppos) await oppo.Distribute();

            isLastDistribute = last;
        });
    }
    public void Distribute(List<int> handCardIds)
    {
        DistributeBase(handCardIds, false);
    }
    public void LastDistribute(List<int> handCardIds)
    {
        DistributeBase(handCardIds, true);
    }

    public void MyThrowResult(ThrowResult throwResult)
    {
        MyPlayer.MyThrowResult(throwResult);
    }
    public void ForcePlay(ThrowResult throwResult)
    {
        MyPlayer.ForceThrow(throwResult);
    }
    public void CurrentOppoThrow(ThrowResult throwResult)
    {
        ((IOppo)PlayerInTurn).Throw(throwResult);
    }

    #endregion

    public void BeginGame(List<int> myHand, List<int> groundCards)
    {
        Ground.I.Distribute(groundCards);

        Distribute(myHand);

        Debug.Log($"hand cards are {string.Join(", ", myHand)}");

        InitialTurn();
    }
}