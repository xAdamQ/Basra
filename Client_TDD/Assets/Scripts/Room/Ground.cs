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
    void Throw(Card thrownCard, List<int> eatenCardsIds, Sequence animSeq, Vector2? meetPoint);
    void Distribute(List<int> cardIds); //tested
}

public class Ground : MonoBehaviour, IGround
{
    [Inject] private Card.Factory _cardFactory;

    private List<Card> Cards { get; } = new List<Card>();
    public static readonly Vector2 Bounds = new Vector2(2f, 2.5f);

    public void Distribute(List<int> cardIds)
    {
        foreach (var cardId in cardIds)
        {
            var card = _cardFactory.CreateGroundCard(cardId, transform);
            Cards.Add(card);
        }

        OrganizeGrid(DOTween.Sequence());
    }

    private static readonly Vector2 GridSize = new Vector2(4, 4);
    private static readonly Vector2 TopLeftAnchor = -Bounds;
    private static readonly Vector2 UnitDistance = new Vector2(1, 2);
    private void OrganizeGrid(Sequence sequence)
    {
        //todo full grid case

        float z = 3;
        var animTime = sequence.Duration(true);
        for (int i = 0; i < Cards.Count; i++)
        {
            var index = new Vector2(i % GridSize.x, i / (int)GridSize.y);
            var poz = new Vector3(TopLeftAnchor.x + index.x * UnitDistance.x, TopLeftAnchor.y + index.y * UnitDistance.y, z);

            Cards[i].transform.position = new Vector3(Cards[i].transform.position.x, Cards[i].transform.position.y, z);
            //because animating z is ugly in 2d world

            sequence.Insert(animTime, Cards[i].transform.DOMove(poz, .5f));
            //todo override the last animation on anim intersection

            var zRot = Random.Range(-15, 15);
            sequence.Insert(animTime, Cards[i].transform.DORotate(new Vector3(0, 180, zRot), .3f));

            z += -.05f;
        }
    }

    private const float EatAnimTime = .5f;

    public void Throw(Card thrownCard, List<int> eatenCardsIds, Sequence animSeq, Vector2? meetPoint)
    {
        if (eatenCardsIds == null || eatenCardsIds.Count == 0)
        {
            Cards.Add(thrownCard);
            OrganizeGrid(animSeq);
            return;
        }

        var eatenCards = Cards.Where(c => eatenCardsIds.Contains(c.Front.Index)).ToList();

        if (eatenCards.Count != eatenCardsIds.Count)
            throw new Exception("the passed eaten cards on the server doesn't exist on the ground");

        eatenCards.ForEach(c => Cards.Remove(c));

        EatAnim(thrownCard, eatenCards, animSeq, meetPoint);

        OrganizeGrid(animSeq);
    }

    private void EatAnim(Card thrownCard, List<Card> eatenCards, Sequence animSeq, Vector2? meetPoint)
    {
        var meetPointValue = meetPoint ?? thrownCard.transform.position;

        var animTime = animSeq.Duration(true);

        eatenCards.ForEach(c => animSeq.Insert(animTime, c.transform.DOMove(meetPointValue, EatAnimTime)));
        eatenCards.ForEach(c => c.transform.transform.position += Vector3.back * 3);

        animTime += EatAnimTime;

        eatenCards.ForEach(c => animSeq.Insert(animTime, c.transform.DOScale(Vector3.zero, .3f)));
        animSeq.InsertCallback(animTime, () => eatenCards.ForEach(c => Destroy(c.gameObject)));

        animSeq.Join(thrownCard.transform.DOScale(Vector3.zero, .3f))
            .AppendCallback(() => Destroy(thrownCard.gameObject));


    }
}