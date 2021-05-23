using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;
using Zenject;

public interface IPlayer : IPlayerBase
{
    UniTask Throw(Card card); //tested
    void Distribute(int[] cardIds); //tested
    void ServerThrow(int cardHandIndex, ThrowResponse throwResponse); //trivial to test
    bool IsPlayable(); //trivial to test
}

public class Player : PlayerBase, IPlayer
{
    [Inject] private readonly TurnTimer _turnTimer;

    public async UniTask Throw(Card card)
    {
        var response = await _controller.ThrowCard(HandCards.IndexOf(card));

        ThrowBase(card, response);
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

    public void Distribute(int[] cardIds)
    {
        foreach (var cardId in cardIds)
        {
            var card = _cardFactory.CreateMyPlayerCard(cardId, transform);
            card.Player = this;
            HandCards.Add(card);
        }

        OrganizeHand();
    }

    public void ServerThrow(int cardHandIndex, ThrowResponse throwResponse)
    {
        ThrowBase(HandCards[cardHandIndex], throwResponse);
    }
}