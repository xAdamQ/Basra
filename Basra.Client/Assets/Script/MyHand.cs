using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyHand : Hand
{
    public void Set(int[] hand)
    {
        for (var i = 0; i < hand.Length; i++)
        {
            MakeCard(hand[i]);
        }

        PlaceCards();
    }

    private Card MakeCard(int id)
    {
        var card = Instantiate(FrequentAssets.I.RealCardPrefab, transform).GetComponent<RealCard>();

        Cards.Add(card);

        card.Id = id;

        card.gameObject.GetComponent<SpriteRenderer>().sprite = FrequentAssets.I.NumberSprites[id];

        return card;
    }
}
