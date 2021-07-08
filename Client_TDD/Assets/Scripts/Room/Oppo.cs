using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using DG.Tweening;

public interface IOppo : IPlayerBase
{
    //tested
    void Throw(ThrowResult throwResult);
    //tested
    void Distribute();
    void Distribute(int cardsCount);
}

public class Oppo : PlayerBase, IOppo
{
    public void Throw(ThrowResult throwResult)
    {
        var randCard = HandCards.GetRandom();
        randCard.AddFront(throwResult.ThrownCard);

        var throwSeq = DOTween.Sequence();

        var targetPoz = PlaceCard(randCard, throwSeq);

        ThrowBase(throwResult, throwSeq, targetPoz);
    }

    public void Distribute()
    {
        Distribute(HandCardCapacity);
    }
    public void Distribute(int cardsCount)
    {
        for (var i = 0; i < cardsCount; i++)
        {
            var card = _cardFactory.CreateOppoCard(transform);
            HandCards.Add(card);
            card.Player = this;
        }

        OrganizeHand();
    }
}