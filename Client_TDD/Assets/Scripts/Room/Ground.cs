using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public interface IGround
{
    
    void Throw(Card thrownCard, int[] eatenCardsIds);//tested
    void InitialDistribute(int[] cardIds);//tested
}

public class Ground : MonoBehaviour, IGround
{
    [Inject] private Card.Factory _cardFactory;

    private List<Card> Cards { get; } = new List<Card>();
    private static readonly Vector2 Bounds = new Vector2(1.5f, 2.5f);

    public void InitialDistribute(int[] cardIds)
    {
        foreach (var cardId in cardIds)
        {
            var card = _cardFactory.CreateGroundCard(cardId, transform);

            Cards.Add(card);
            PlaceCard(card);
        }
    }

    private void PlaceCard(Card card)
    {
        card.transform.eulerAngles = new Vector3(0, 180, 0);

        var xPoz = Random.Range(-Bounds.x, Bounds.x);
        var yPoz = Random.Range(-Bounds.y, Bounds.y);
        card.transform.DOMove(new Vector3(xPoz, yPoz), .75f);
    }

    public void Throw(Card thrownCard, int[] eatenCardsIds)
    {
        if (eatenCardsIds.Length == 0)
        {
            Cards.Add(thrownCard);
            PlaceCard(thrownCard);

            return;
        }

        var eatenCards = Cards.Where(c => eatenCardsIds.Contains(c.Front.Index)).ToArray();

        if (eatenCards.Length != eatenCardsIds.Length)
            throw new Exception("the passed eaten cards on the server doesn't exist on the ground");

        foreach (var eatenCard in eatenCards)
        {
            Cards.Remove(eatenCard);

            eatenCard.transform.DOMove(thrownCard.transform.position, .5f)
                .OnComplete(() =>
                {
                    Destroy(eatenCard.gameObject);

                    thrownCard.transform.DOScale(Vector3.zero, .5f)
                        .OnComplete(() => Destroy(thrownCard.gameObject));
                });
        }
    }
}