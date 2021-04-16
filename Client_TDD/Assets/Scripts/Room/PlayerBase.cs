using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IPlayerBase
{
}

public abstract class PlayerBase : MonoBehaviour
{
    public const int Size = 4;
    public const int HandTime = 8000;

    public List<ICard> Cards { get; } = new List<ICard>();
    public int TurnId { get; }

    protected Card.Factory _cardFactory;

    [Inject]
    public void Construct(Card.Factory cardFactory)
    {
        _cardFactory = cardFactory;
    }

    protected void OrganizeHand()
    {
        throw new NotImplementedException();
    }

    public void EnterTurn()
    {
    }
}