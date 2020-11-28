using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public List<Card> Cards, PrevCards;
    public Card PrevCard;

    public static Vector2 Bounds = new Vector2(1.5f, 2.5f);
    public RoomManager Room;
    private Card ProcessingCard;

    public void Set(int[] ground)
    {
        for (var i = 0; i < ground.Length; i++)
        {
            var card = MakeCard(ground[i]);
            Add(card);
        }
    }

    public Card MakeCard(int id)
    {
        var card = Instantiate(FrequentAssets.I.CardPrefab, transform).GetComponent<Card>();
        card.AddFront(id);
        return card;
    }

    private void PlaceCard(Card card)
    {
        var xPoz = Random.Range(-Bounds.x, Bounds.x);
        var yPoz = Random.Range(-Bounds.y, Bounds.y);
        card.transform.position = new Vector3(xPoz, yPoz);
    }

    public void Add(Card card)
    {
        PrevCards = new List<Card>(Cards);
        PrevCard = Instantiate(card);
        PrevCard.gameObject.SetActive(false);

        Cards.Add(card);
        PlaceCard(card);
        card.Type = CardType.Ground;
    }
    public void ReversAddIF(Card card)
    {
        Cards = new List<Card>(PrevCards);
        Destroy(card);
        card = PrevCard;
        card.gameObject.SetActive(true);
    }

    public void AddIF(Card card)
    {
        Cards.Add(card);

        PlaceCard(card);

        card.Type = CardType.Ground;
    }

    public void ShadowAdd(Card card)
    {
        ProcessingCard = card;
        PlaceCard(ProcessingCard);
    }
    public void ConfirmAdd()
    {
        Cards.Add(ProcessingCard);
        ProcessingCard.Type = CardType.Ground;
    }
    public void ReverseAdd()
    {
        //return card back to it's position, so you could memorized the state of it
        //memorizing is the key for a time machine

    }

    public void VisualAdd(Card card)
    {
        AppManager.I.LastAction.RecoredTransform(card);
        PlaceCard(card);
    }
    public void ActualAdd(Card card)
    {

    }


}