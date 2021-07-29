using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Basra.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;

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

    //todo test this
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

    protected static readonly int HandCardCapacity = 4;

    private void Awake()
    {
        turnTimer = GetComponent<TurnTimer>();
    }

    protected virtual void Start()
    {
        HandCenter = ((endCard.position - startCard.position) / 2) + startCard.position;

        turnTimer.Ticked += RoomUserView.Manager.I.RoomUserViews[Turn].SetTurnFill;
    }

    private async UniTask Init(int selectedBackIndex, int turn)
    {
        //todo this will be different with the full back sprite set
        //i suggest array of sprite addresses

        // var backSprite = await Addressables.LoadAssetAsync<Sprite>($"cardbackSprites[0_{selectedBackIndex}]");
        // BackSprite = backSprite;
        Turn = turn;

        await Extensions.LoadAndReleaseAsset<Sprite>(((CardbackType) selectedBackIndex).ToString(),
            sprite => BackSprite = sprite);
    }

    private int Turn { get; set; }

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
            .Join(card.transform.DORotate(new Vector3(0, 180), .3f).SetDelay(.2f));

        return targetPoz;
    }

    public async UniTask EatLast()
    {
        UpdateEatStatus(Ground.I.Cards.Count, false, false);

        var sequence = DOTween.Sequence();

        await Ground.I.EatLast(HandCenter, sequence);
    }

    protected void ThrowBase(ThrowResult result, Sequence animSeq = null, Vector2? meetPoint = null)
    {
        animSeq ??= DOTween.Sequence();

        var card = HandCards.First(c => c.Front != null && c.Front.Index == result.ThrownCard);

        Ground.I.Throw(card, result.EatenCardsIds, animSeq, meetPoint);

        var count = result.EatenCardsIds?.Count ?? 0;
        UpdateEatStatus(count, result.Basra, result.BigBasra);

        HandCards.Remove(card);
        OrganizeHand();

        CoreGameplay.I.NextTurn();
    }

    private void UpdateEatStatus(int eatenCount, bool basra, bool bbasra)
    {
        eatenCount += eatenCount;
        eatenText.text = eatenCount.ToString();
        if (basra) AddBasra();
        if (bbasra) AddBigBasra();
    }

    #region player ui

    private int eatenCount;
    [SerializeField] private TextMesh eatenText;

    [SerializeField] private GameObject basraSymbol, bigBasraSymbol;
    [SerializeField] private TextMesh basraText, bigBasraText;
    private int basraCount, bigBasraCount;

    private void AddBasra()
    {
        basraCount++;
        basraSymbol.SetActive(true);
        basraText.text = basraCount.ToString();
        if (basraCount != 1) basraText.gameObject.SetActive(true);
    }
    private void AddBigBasra()
    {
        bigBasraCount++;
        bigBasraSymbol.SetActive(true);
        bigBasraText.text = bigBasraCount.ToString();
        if (bigBasraCount != 1) bigBasraText.gameObject.SetActive(true);
    }

    #endregion

    protected void OrganizeHand()
    {
        var pointer = startCard.localPosition;

        var handSize = endCard.localPosition.x - startCard.localPosition.x;
        var spacing = new Vector3(handSize / (HandCards.Count - 1), 0, .05f);

        if (this is IPlayer)
            HandCards.ForEach(card => card.transform.eulerAngles = Vector3.up * 180);

        foreach (var card in HandCards)
        {
            card.transform.DOLocalMove(pointer, .75f);
            pointer += spacing;
        }
    }

    public virtual void StartTurn()
    {
        turnTimer.Play();
    }
    public virtual void EndTurn()
    {
        turnTimer.Stop();
        RoomUserView.Manager.I.RoomUserViews[Turn].SetTurnFill(0);
    }


    public static async UniTask<PlayerBase> Create(int selectedCardback, int placeIndex, int turn)
    {
        var player = (await Addressables.InstantiateAsync($"player{placeIndex}", RoomReferences.I.Root)).GetComponent<PlayerBase>();

        await player.Init(selectedCardback, turn);


        return player;
    }
}