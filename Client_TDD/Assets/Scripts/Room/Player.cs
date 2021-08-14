using System.Collections.Generic;
using System.Linq;
using Basra.Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public interface IPlayer : IPlayerBase
{
    void Throw(Card card); //tested
    UniTask Distribute(List<int> cardIds); //tested
    void ForceThrow(ThrowResult throwResult); //trivial to test
    bool IsPlayable { get; set; } //trivial to test
    void MyThrowResult(ThrowResult result);
}

public class Player : PlayerBase, IPlayer
{
    public bool IsPlayable { get; set; }

    protected override void Start()
    {
        base.Start();

        turnTimer.Elapsed += MissTurn;
    }

    public async UniTask Distribute(List<int> cardIds)
    {
        foreach (var cardId in cardIds)
        {
            var card = await Card.CreateMyPlayerCard(cardId, BackSprite, transform);
            card.Player = this;
            HandCards.Add(card);
        }

        DistributeAnim();
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
            card.transform.eulerAngles = new Vector3(0, 0, Random.Range(-Card.RotBound, Card.RotBound));
            card.transform.DOScale(Vector3.one, .7f);
            card.transform.DOLocalMove(pointer, .5f);

            pointer += spacing;
        }

        HandCards.ForEach(
            card => card.transform.DORotate(card.transform.eulerAngles.SetY(180), .4f)
            .OnComplete(() => Destroy(card.GetComponent<SpriteRenderer>()))
            .SetDelay(.4f));
    }

    public void Throw(Card card)
    {
        IsPlayable = false;

        //no await for return style because of server inability to do following actions
        Controller.I.ThrowCard(HandCards.IndexOf(card));

        turnTimer.Stop();
    }

    public void MyThrowResult(ThrowResult result)
    {
        ThrowBase(result);
    }

    public override void StartTurn()
    {
        base.StartTurn();

        HandCards.ForEach(c => c.Front.GetComponent<SpriteRenderer>().color = Color.white);

        IsPlayable = true;
    }

    public override void EndTurn()
    {
        base.EndTurn();
        HandCards.ForEach(c =>
        {
            // c.GetComponent<SpriteRenderer>().color = Color.clear;
            c.Front.GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f, .7f);
        });
    }

    private void MissTurn()
    {
        IsPlayable = false;

        Controller.I.NotifyTurnMiss().Forget(e => throw e);
    }

    public void ForceThrow(ThrowResult throwResult)
    {
        IsPlayable = false; //redundant i think

        var card = HandCards.First(c => c.Front.Index == throwResult.ThrownCard);

        var throwSeq = DOTween.Sequence();

        var targetPoz = PlaceCard(card, throwSeq);

        ThrowBase(throwResult, throwSeq, targetPoz);
    }
}