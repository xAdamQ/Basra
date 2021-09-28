using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

public interface IGround
{
    void Throw(Card thrownCard, List<int> eatenCardsIds, Sequence animSeq, Vector2? meetPoint);
    void Distribute(List<int> cardIds); //tested
    List<Card> Cards { get; }
    Vector3 LeftBottomBound { get; }
    Vector3 TopRightBound { get; }
    UniTask EatLast(Vector2 meetPoint, Sequence sequence);
}

public class Ground : MonoBehaviour, IGround
{
    public static async UniTask Create()
    {
        I = (await Addressables.InstantiateAsync("ground", RoomReferences.I.Root))
            .GetComponent<Ground>();
    }

    public static IGround I;

    public List<Card> Cards { get; } = new List<Card>();

    public void Distribute(List<int> cardIds)
    {
        UniTask.Create(async () =>
        {
            foreach (var cardId in cardIds)
            {
                var card = await Card.CreateGroundCard(cardId, transform);
                card.transform.eulerAngles = Vector3.up * 180;
                Cards.Add(card);
            }

            DistributeAnim(DOTween.Sequence());
        });
    }


    [SerializeField] private Transform leftBottomBound, topRightBound;

    public Vector3 LeftBottomBound { get; private set; }
    public Vector3 TopRightBound { get; private set; }

    private Vector2Int GridSize = new Vector2Int(3, 4);

    private void Awake()
    {
        LeftBottomBound = leftBottomBound.position;
        TopRightBound = topRightBound.position;
    }

    private void OrganizeGrid(Sequence sequence)
    {
        if (Cards.Count == 0 || Cards.Count == 1) return;

        var z = 3f;
        var animTime = sequence.Duration();

        GridSize.x = Mathf.RoundToInt(Mathf.Sqrt(Cards.Count));
        GridSize.y = Mathf.CeilToInt(Cards.Count / (float)GridSize.x);

        var spacing = new Vector3(
            (topRightBound.position.x - leftBottomBound.position.x) / (GridSize.x),
            (topRightBound.position.y - leftBottomBound.position.y) / (GridSize.y),
            .1f);

        // var xSize = Cards.Count / 2;
        // if (xSize == 0) return; //ground is empty
        // var ySize = xSize + Cards.Count % xSize;
        //
        // var pointer = leftBottomBound.position;
        //
        // var linearIndex = 0;
        //
        // for (int y = 0; y < ySize; y++)
        // {
        //     pointer.y += unitDistance.y;
        //
        //     for (int x = 0; x < xSize; x++)
        //     {
        //         if (linearIndex >= Cards.Count) break;
        //
        //         pointer.x += unitDistance.x;
        //         pointer.z += unitDistance.z;
        //
        //         Cards[linearIndex].transform.position = pointer;
        //         linearIndex++;
        //         Debug.Log(pointer);
        //     }
        //     pointer.x = leftBottomBound.position.x;
        // }

        for (int i = 0; i < Cards.Count; i++)
        {
            var index = new Vector2Int(i % GridSize.x, i / GridSize.x);

            var poz = new Vector3(leftBottomBound.position.x + index.x * spacing.x,
                leftBottomBound.position.y + index.y * spacing.y, index.y + z);

            Cards[i].transform.position = new Vector3(Cards[i].transform.position.x,
                Cards[i].transform.position.y, z);
            //because animating z is ugly in 2d world

            sequence.Insert(animTime, Cards[i].transform.DOMove(poz, .5f));
            //todo override the last animation on anim intersection

            z += -.05f;
        }
    }


    private void DistributeAnim(Sequence sequence)
    {
        var animTime = sequence.Duration();

        OrganizeGrid(sequence);

        foreach (var card in Cards)
        {
            sequence.Insert(animTime, card.transform.DOScale(Vector3.one, .7f));
            sequence.Insert(animTime,
                card.transform.DORotate(
                    new Vector3(0, 180, Random.Range(-Card.RotBound, Card.RotBound)), .3f));
        }
    }


    private const float EatAnimTime = .5f;

    public void Throw(Card thrownCard, List<int> eatenCardsIds, Sequence animSeq,
        Vector2? meetPoint)
    {
        thrownCard.Player = null;

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

    private void EatAnim(Card thrownCard, List<Card> eatenCards, Sequence animSeq,
        Vector2? meetPoint)
    {
        var meetPointValue = meetPoint ?? thrownCard.transform.position;

        var animTime = animSeq.Duration();

        eatenCards.ForEach(c =>
            animSeq.Insert(animTime, c.transform.DOMove(meetPointValue, EatAnimTime)));
        eatenCards.ForEach(c => c.transform.transform.position += Vector3.back * 3);

        animTime += EatAnimTime;

        eatenCards.ForEach(c => animSeq.Insert(animTime, c.transform.DOScale(Vector3.zero, .3f)));
        animSeq.InsertCallback(animTime, () => eatenCards.ForEach(c => Destroy(c.gameObject)));

        animSeq.Join(thrownCard.transform.DOScale(Vector3.zero, .3f))
            .AppendCallback(() => Destroy(thrownCard.gameObject));
    }

    public async UniTask EatLast(Vector2 meetPoint, Sequence animSeq)
    {
        if (Cards.Count == 0) return;

        var animTime = animSeq.Duration();

        Cards.ForEach(c => animSeq.Insert(animTime, c.transform.DOMove(meetPoint, EatAnimTime)));

        animTime += EatAnimTime;

        Cards.ForEach(c => animSeq.Insert(animTime, c.transform.DOScale(Vector3.zero, .3f)));
        animSeq.InsertCallback(animTime, () => Cards.ForEach(c => Destroy(c.gameObject)));

        await UniTask.Delay((int)(animSeq.Duration() * 1000));

        Cards.Clear();
    }
}