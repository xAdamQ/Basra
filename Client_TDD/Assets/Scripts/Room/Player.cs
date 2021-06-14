using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;
using Zenject;

public interface IPlayer : IPlayerBase
{
    void Throw(Card card); //tested
    void Distribute(List<int> cardIds); //tested
    void ServerThrow(ThrowResult throwResult); //trivial to test
    bool IsPlayable(); //trivial to test
    void MyThrowResult(ThrowResult result);
}

public class Player : PlayerBase, IPlayer
{
    [Inject] private readonly TurnTimer _turnTimer;


    public void Throw(Card card)
    {
        //no await for return style because of server inability to do following actions
        _controller.ThrowCard(HandCards.IndexOf(card));
        //blocking here is custom
        // ThrowBase(card, response);
    }

    public void MyThrowResult(ThrowResult result)
    {
        ThrowBase(result);
    }

    private void Start()
    {
        _turnTimer.uniTaskTimer.Elapsed += MissTurn;
    }

    private void MissTurn()
    {
        _controller.NotifyTurnMiss(); //async forgotten
    }

    public bool IsPlayable()
    {
        return _roomController.CurrentTurn == Turn && _turnTimer.uniTaskTimer.Active;
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

    public void ServerThrow(ThrowResult throwResult)
    {
        ThrowBase(throwResult);
    }
}