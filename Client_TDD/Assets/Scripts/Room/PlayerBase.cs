using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Basra.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public interface IPlayerBase
{
    void EndTurn();
    void StartTurn();
    UniTask EatLast();
    List<Card> HandCards { get; }
}

public abstract class PlayerBase : MonoBehaviour, IPlayerBase
{
    [SerializeField] protected Transform startCard, endCard;

    private static AudioClip throwClip, turnClip;
    private static bool InitOnce; //be aware this is once in the whole game, even after scene reload

    public static int ConvertTurnToPlace(int turn, int myTurn)
    {
        if (myTurn == turn) return 0;

        if (turn < myTurn) return turn + 1;

        return turn;

        //
        // var oppoPlaceCounter = 1;
        // for (int i = 0; i < capacity; i++)
        //     if (myTurn != i)
        //         oppoPlaceCounter++;
        // return oppoPlaceCounter;
    }

    protected Sprite BackSprite;

    protected const int HandCardCapacity = 4;

    private void Awake()
    {
        turnTimer = GetComponent<TurnTimer>();

        if (InitOnce) return;

        InitOnce = true;

        Addressables.LoadAssetAsync<AudioClip>("throwClip").Completed +=
            handle => throwClip = handle.Result;
        Addressables.LoadAssetAsync<AudioClip>("turnClip").Completed +=
            handle => turnClip = handle.Result;
    }

    protected virtual void Start()
    {
        HandCenter = ((endCard.position - startCard.position) / 2) + startCard.position;

        turnTimer.Ticked += RoomUserView.Manager.I.RoomUserViews[Turn].SetTurnFill;
    }

    private async UniTask Init(int selectedBackIndex, int turn)
    {
        Turn = turn;

        await Extensions.LoadAndReleaseAsset<Sprite>(((CardbackType)selectedBackIndex).ToString(),
            sprite => BackSprite = sprite);
    }

    protected int Turn { get; set; }

    public List<Card> HandCards { get; } = new List<Card>();

    protected TurnTimer turnTimer;

    public Vector2 HandCenter { private set; get; }

    protected Vector3 PlaceCard(Card card, Sequence animSeq)
    {
        var targetPoz = new Vector3(
            Random.Range(Ground.I.LeftBottomBound.x, Ground.I.TopRightBound.x),
            Random.Range(Ground.I.LeftBottomBound.y, Ground.I.TopRightBound.y),
            -1);

        animSeq
            .Append(card.transform.DOMove(targetPoz, .5f))
            .Join(card.transform.DORotate(card.transform.eulerAngles.SetY(180f), .3f)
                .SetDelay(.2f));

        return targetPoz;
    }

    public async UniTask EatLast()
    {
        var sequence = DOTween.Sequence();

        await Ground.I.EatLast(HandCenter, sequence);
    }

    protected void ThrowBase(ThrowResult result, Sequence animSeq = null, Vector2? meetPoint = null)
    {
        AudioManager.I.Play(throwClip);
        animSeq ??= DOTween.Sequence();

        var card = HandCards.First(c => c.Front != null && c.Front.Index == result.ThrownCard);

        Ground.I.Throw(card, result.EatenCardsIds, animSeq, meetPoint);

        var count = result.EatenCardsIds?.Count ?? 0;

        HandCards.Remove(card);
        OrganizeHand();

        CoreGameplay.I.NextTurn();
    }

    private void OrganizeHand()
    {
        var pointer = startCard.localPosition;

        var handSize = endCard.localPosition.x - startCard.localPosition.x;
        var spacing = new Vector3(handSize / (HandCards.Count + 1), 0, .05f);

        foreach (var card in HandCards)
        {
            pointer += spacing;
            card.transform.DOLocalMove(pointer, .75f);
        }
    }

    public virtual void StartTurn()
    {
        // AudioManager.I.PlayIfSilent(turnClip);

        turnTimer.Play();
    }
    public virtual void EndTurn()
    {
        turnTimer.Stop();
        RoomUserView.Manager.I.RoomUserViews[Turn].SetTurnFill(1);
    }

    public static async UniTask<PlayerBase> Create(int selectedCardback, int placeIndex, int turn)
    {
        var player =
            (await Addressables.InstantiateAsync($"player{placeIndex}", RoomReferences.I.Root))
            .GetComponent<PlayerBase>();

        await player.Init(selectedCardback, turn);


        return player;
    }
}