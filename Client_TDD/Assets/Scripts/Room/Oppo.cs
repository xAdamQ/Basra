using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public interface IOppo : IPlayerBase
{
    //tested
    void Throw(int cardId, ThrowResponse throwResponse);
    //tested
    void Distribute();
}

public class Oppo : PlayerBase, IOppo
{
    public void Throw(int cardId, ThrowResponse throwResponse)
    {
        var randCard = HandCards.GetRandom();
        randCard.AddFront(cardId);
        ThrowBase(randCard, throwResponse);
    }

    public void Distribute()
    {
        for (var i = 0; i < HandSize; i++)
        {
            var card = _cardFactory.CreateOppoCard(transform);
            HandCards.Add(card);
            card.Player = this;
        }

        OrganizeHand();
    }
}