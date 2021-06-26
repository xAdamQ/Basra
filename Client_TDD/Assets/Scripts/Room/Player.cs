using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.XR;
using Zenject;

public interface IPlayer : IPlayerBase
{
    void Throw(Card card); //tested
    void Distribute(List<int> cardIds); //tested
    void ForceThrow(ThrowResult throwResult); //trivial to test
    bool IsPlayable(); //trivial to test
    void MyThrowResult(ThrowResult result);
}

public class Player : PlayerBase, IPlayer
{
    [Inject] private readonly IController _controller;
    [Inject] private readonly ITurnTimer _turnTimer;

    private bool IsNormalThrowCalled;

    public void Throw(Card card)
    {
        //no await for return style because of server inability to do following actions
        IsNormalThrowCalled = true;
        _turnTimer.Elapsed -= MissTurn;

        _controller.ThrowCard(HandCards.IndexOf(card));
    }

    public void ForceThrow(ThrowResult throwResult)
    {
        var card = HandCards.First(c => c.Front.Index == throwResult.ThrownCard);

        OrganizeHand(); //return the processing card if any

        var throwSeq = DOTween.Sequence();

        var targetPoz = PlaceCard(card, throwSeq);

        ThrowBase(throwResult, throwSeq, targetPoz);
    }

    public void MyThrowResult(ThrowResult result)
    {
        ThrowBase(result);
    }

    public bool IsPlayable()
    {
        return _coreGameplay.PlayerInTurn == this && _turnTimer.IsPlaying;
    }

    public void Distribute(List<int> cardIds)
    {
        foreach (var cardId in cardIds)
        {
            var card = _cardFactory.CreateMyPlayerCard(cardId, transform);
            card.Player = this;
            HandCards.Add(card);
        }

        OrganizeHand();
    }

    public override void StartTurn()
    {
        base.StartTurn();
        IsNormalThrowCalled = false;
        _turnTimer.Elapsed += MissTurn;
    }

    public override void EndTurn()
    {
        base.EndTurn();

        if (!IsNormalThrowCalled)
            _turnTimer.Elapsed -= MissTurn;
    }

    private void MissTurn()
    {
        _controller.NotifyTurnMiss().Forget(e => throw e);
    }

}