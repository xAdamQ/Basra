using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public interface IPlayerBase
{
    void EndTurn();
    void StartTurn();
    UniTask EatLast();
    List<Card> HandCards { get; }
    UniTask Init(int selectedBackIndex);
}

public abstract class PlayerBase : MonoBehaviour, IPlayerBase
{
    [SerializeField] private Transform startCard, endCard;

    [Inject] protected readonly Card.Factory _cardFactory;
    [Inject] protected readonly IGround _ground;
    [Inject] protected readonly ICoreGameplay _coreGameplay;
    [Inject] protected readonly RoomUserView.IManager _ruvManager;

    protected Sprite BackSprite;

    protected static readonly int HandCardCapacity = 4;

    private void Awake()
    {
        turnTimer = GetComponent<TurnTimer>();
    }

    protected virtual void Start()
    {
        HandCenter = ((endCard.position - startCard.position) / 2) + startCard.position;

        turnTimer.Ticked += _ruvManager.RoomUserViews[Turn].SetTurnFill;
    }

    public async UniTask Init(int selectedBackIndex)
    {
        //todo this will be different with the full back sprite set
        //i suggest array of sprite addresses
        var backSprite = await Addressables.LoadAssetAsync<Sprite>($"cardbackSprites[0_{selectedBackIndex}]");

        BackSprite = backSprite;
    }

    private int Turn { get; set; }

    public List<Card> HandCards { get; } = new List<Card>();

    protected TurnTimer turnTimer;

    public Vector2 HandCenter { private set; get; }

    protected Vector3 PlaceCard(Card card, Sequence animSeq)
    {
        var targetPoz = new Vector3(
            Random.Range(_ground.LeftBottomBound.x, _ground.TopRightBound.x),
            Random.Range(_ground.LeftBottomBound.y, _ground.TopRightBound.y));

        animSeq.Append(card.transform.DOMove(targetPoz, .5f))
            .Join(card.transform.DORotate(new Vector3(0, 180), .3f));

        return targetPoz;
    }

    public async UniTask EatLast()
    {
        UpdateEatStatus(_ground.Cards.Count, false, false);

        var sequence = DOTween.Sequence();

        await _ground.EatLast(HandCenter, sequence);
    }

    protected void ThrowBase(ThrowResult result, Sequence animSeq = null, Vector2? meetPoint = null)
    {
        animSeq ??= DOTween.Sequence();

        var card = HandCards.First(c => c.Front != null && c.Front.Index == result.ThrownCard);

        _ground.Throw(card, result.EatenCardsIds, animSeq, meetPoint);

        var count = result.EatenCardsIds?.Count ?? 0;
        UpdateEatStatus(count, result.Basra, result.BigBasra);

        HandCards.Remove(card);
        OrganizeHand();

        _coreGameplay.NextTurn();
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
        _ruvManager.RoomUserViews[Turn].SetTurnFill(0);
    }

    public class Factory
    {
        private readonly IInstantiator _instantiator;
        private readonly GameObject[] _playerPrefabs;

        [Inject]
        public Factory(IInstantiator instantiator, GameObject[] playerPrefabs)
        {
            _instantiator = instantiator;
            _playerPrefabs = playerPrefabs;
        }

        public async UniTask<PlayerBase> Create(int selectedCardback, int placeIndex, int turn)
        {
            var player = _instantiator.InstantiatePrefab(_playerPrefabs[placeIndex]).GetComponent<PlayerBase>();

            await player.Init(selectedCardback);

            player.Turn = turn;

            return player;
        }
    }
}