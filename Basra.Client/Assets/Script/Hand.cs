using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<Card> Cards = new List<Card>();
    public static int Size = 4;

    public RoomManager Room;

    //act on all cards because we don't add inidie cards
    protected void PlaceCards()
    {
        var xPointer = -(Size / 2) * Card.Bounds.x;
        xPointer -= xPointer / 2f;

        var xSpacing = Card.Bounds.x;

        for (var i = 0; i < Cards.Count; i++)
        {
            Cards[i].transform.localPosition = Vector3.right * xPointer;
            xPointer += xSpacing;
        }
    }

    //my
    public void Set(int[] hand)
    {
        for (var i = 0; i < hand.Length; i++)
        {
            var card = MakeCard(hand[i]);
            card.Type = CardType.Mine;
        }
        PlaceCards();
    }
    //oppo
    public void Set()
    {
        for (var i = 0; i < Size; i++)
        {
            MakeCard();
        }
        PlaceCards();
    }

    //my
    private Card MakeCard(int id)
    {
        var card = Instantiate(FrequentAssets.I.CardPrefab, transform).GetComponent<Card>();
        card.AddFront(id);
        Cards.Add(card);
        return card;
    }
    //oppo
    private Card MakeCard()
    {
        var card = Instantiate(FrequentAssets.I.CardPrefab, transform).GetComponent<Card>();
        Cards.Add(card);
        return card;
    }

}
