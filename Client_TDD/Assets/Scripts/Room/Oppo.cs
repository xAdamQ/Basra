using System;
using System.Collections;
using System.Collections.Generic;
using Basra.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public interface IOppo : IPlayerBase
{
    //tested
    void Throw(ThrowResult throwResult);
    //tested
    UniTask Distribute();
    UniTask Distribute(int cardsCount);
}

public class Oppo : PlayerBase, IOppo
{
    public void Throw(ThrowResult throwResult)
    {
        UniTask.Create(async () =>
        {
            var randCard = HandCards.GetRandom();
            await randCard.AddFront(throwResult.ThrownCard);

            var throwSeq = DOTween.Sequence();

            var targetPoz = PlaceCard(randCard, throwSeq);

            ThrowBase(throwResult, throwSeq, targetPoz);
        });
    }

    private void DistributeAnim()
    {
        HandCards.ForEach(c => c.transform.position = Vector2.Lerp(startCard.position, endCard.position, .5f));
        //the start anim position

        var pointer = startCard.localPosition;

        var handSize = endCard.localPosition.x - startCard.localPosition.x;
        var spacing = new Vector3(handSize / (HandCards.Count - 1), 0, .05f);

        foreach (var card in HandCards)
        {
            card.transform.DOScale(Vector3.one, .7f);
            card.transform.DOLocalMove(pointer, .5f);

            pointer += spacing;
        }
    }

    public async UniTask Distribute()
    {
        await Distribute(HandCardCapacity);
    }
    public async UniTask Distribute(int cardsCount)
    {
        for (var i = 0; i < cardsCount; i++)
        {
            var card = await Card.CreateOppoCard(BackSprite, transform);
            HandCards.Add(card);
            card.Player = this;
        }

        DistributeAnim();
    }
}