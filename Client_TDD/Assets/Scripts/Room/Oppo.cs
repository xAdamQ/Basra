using System;
using Basra.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
        HandCards.ForEach(c =>
            c.transform.position = Vector2.Lerp(startCard.position, endCard.position, .5f));
        //the start anim position

        var pointer = startCard.localPosition;

        var handSize = endCard.localPosition.x - startCard.localPosition.x;
        var spacing = new Vector3(handSize / (HandCards.Count - 1), 0, .05f);

        foreach (var card in HandCards)
        {
            card.transform.DOScale(Vector3.one, .7f);
            card.transform.DOLocalMove(pointer, .5f);
            card.transform.DORotate(
                new Vector3(0, 180, Random.Range(-Card.RotBound, Card.RotBound)), .3f);

            pointer += spacing;
        }
    }

    public override void StartTurn()
    {
        base.StartTurn();
        RoomUserView.Manager.I.RoomUserViews[Turn].TurnFocus(true);
    }

    public override void EndTurn()
    {
        base.EndTurn();
        RoomUserView.Manager.I.RoomUserViews[Turn].TurnFocus(false);
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