using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using UnityEngine.XR;
using System.Linq;

public interface IPlayerBase
{
    void EndTurn();
    void StartTurn();
}

public abstract class PlayerBase : MonoBehaviour, IPlayerBase
{
    #region user places

    private const float UpperPadding = 1.5f, BottomPadding = 1f;

    private static readonly Vector2[] UserPositions =
    {
        new Vector2(-.3f, -5),
        new Vector2(0, 6),
        new Vector2(3.5f, -2.8f),
        new Vector2(-3.5f, 3.7f),
    };

    private static readonly Vector3[] UserRotations =
    {
        new Vector3(),
        new Vector3(0, 0, 180),
        new Vector3(0, 0, 90),
        new Vector3(0, 0, -90),
    };

    #endregion


    [Inject] protected readonly Card.Factory _cardFactory;
    [Inject] protected readonly IGround _ground;
    [Inject] protected readonly ICoreGameplay _coreGameplay;




    protected static readonly int HandCardCapacity = 4;
    protected static readonly Vector2 HandXBounds = new Vector2(.45f, 2.2f);
    protected static readonly float HandSize = HandXBounds.y - HandXBounds.x;

    protected List<Card> HandCards { get; } = new List<Card>();

    protected Vector3 PlaceCard(Card card, Sequence animSeq)
    {
        var targetPoz = new Vector3(
            UnityEngine.Random.Range(-Ground.Bounds.x, Ground.Bounds.x),
            UnityEngine.Random.Range(-Ground.Bounds.y, Ground.Bounds.y));

        animSeq.
            Append(card.transform.DOMove(targetPoz, .5f))
            .Join(card.transform.DORotate(new Vector3(0, 180), .3f));

        return targetPoz;
    }

    protected void ThrowBase(ThrowResult result, Sequence animSeq = null, Vector2? meetPoint = null)
    {
        animSeq ??= DOTween.Sequence();

        var card = HandCards.First(c => c.Front != null && c.Front.Index == result.ThrownCard);

        _ground.Throw(card, result.EatenCardsIds, animSeq, meetPoint);

        if (result.EatenCardsIds != null) eatenCount += result.EatenCardsIds.Count;
        eatenText.text = eatenCount.ToString();

        if (result.Basra) AddBasra();
        if (result.BigBasra) AddBigBasra();

        HandCards.Remove(card);
        OrganizeHand();

        _coreGameplay.NextTurn();
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
        // var pointer = new Vector3(-(HandCardCapacity / 2) * Card.Bounds.x, 0, 0);
        // pointer.x -= pointer.x / 2f;
        var pointer = new Vector3(HandXBounds.x, 0, 0);

        var spacing = new Vector3(HandSize / HandCards.Count, 0, .05f);

        if (this is IPlayer)
            HandCards.ForEach(card => card.transform.eulerAngles = Vector3.up * 180);

        foreach (var card in HandCards)
        {
            card.transform.DOLocalMove(pointer, .75f);
            pointer += spacing;
        }
    }

    [SerializeField] GameObject TurnIndicator;
    public virtual void StartTurn()
    {
        TurnIndicator.SetActive(true);
    }
    public virtual void EndTurn()
    {
        TurnIndicator.SetActive(false);
    }

    public class Factory
    {
        private readonly IInstantiator _instantiator;
        private readonly GameObject[] _playerPrefabs;

        [Inject]
        public Factory(IInstantiator instantiator, GameObject[] playerPrefabs)
        {
            _instantiator = instantiator;
        }

        public PlayerBase Create(PlayerType playerType, int turn)
        {
            var player = _instantiator.InstantiatePrefab(_playerPrefabs[turn]).GetComponent<PlayerBase>();

            player.transform.position = UserPositions[turn];
            player.transform.eulerAngles = UserRotations[turn];

            return player;
        }
    }
}