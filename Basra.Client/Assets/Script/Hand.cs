using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hand : MonoBehaviour
{
    protected List<Card> Cards = new List<Card>();
    public static int Size = 4;

    protected void PlaceCards()
    {
        var xPointer = -Size * Card.Bounds.x;
        var xSpacing = Card.Bounds.x;

        for (var i = 0; i < Cards.Count; i++)
        {
            Cards[i].transform.localPosition = Vector3.right * xPointer;
            xPointer += xSpacing;
        }
    }

    public void Play(int cardIndex)
    {

    }
}
