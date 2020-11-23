using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherHand : Hand
{
    public void Set()
    {
        for (var i = 0; i < Size; i++)
        {
            MakeCard();
        }
        PlaceCards();
    }

    private Card MakeCard()
    {
        var card = Instantiate(FrequentAssets.I.EmptyCardPrefab, transform).GetComponent<EmptyCard>();

        Cards.Add(card);

        return card;
    }
}
