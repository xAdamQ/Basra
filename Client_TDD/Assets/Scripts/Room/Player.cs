using System;
using System.Collections;
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
    bool IsPlayable { get; set; } //trivial to test
    void MyThrowResult(ThrowResult result);
}

public class Player : PlayerBase, IPlayer
{
    [Inject] private readonly IController _controller;

    public bool IsPlayable { get; set; }

    protected override void Start()
    {
        base.Start();

        turnTimer.Elapsed += MissTurn;
    }

    public void Distribute(List<int> cardIds)
    {
        foreach (var cardId in cardIds)
        {
            var card = _cardFactory.CreateMyPlayerCard(cardId, BackSprite, transform);
            card.Player = this;
            HandCards.Add(card);
        }

        OrganizeHand();
    }

    public void Throw(Card card)
    {
        IsPlayable = false;

        //no await for return style because of server inability to do following actions
        _controller.ThrowCard(HandCards.IndexOf(card));

        turnTimer.Stop();
    }

    public void MyThrowResult(ThrowResult result)
    {
        ThrowBase(result);
    }

    public override void StartTurn()
    {
        base.StartTurn();

        IsPlayable = true;
    }

    private void MissTurn()
    {
        IsPlayable = false;

        _controller.NotifyTurnMiss().Forget(e => throw e);
    }

    public void ForceThrow(ThrowResult throwResult)
    {
        IsPlayable = false; //redundant i think

        var card = HandCards.First(c => c.Front.Index == throwResult.ThrownCard);

        var throwSeq = DOTween.Sequence();

        var targetPoz = PlaceCard(card, throwSeq);

        ThrowBase(throwResult, throwSeq, targetPoz);
    }
}