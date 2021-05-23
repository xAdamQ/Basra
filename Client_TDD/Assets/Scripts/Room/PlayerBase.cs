using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using UnityEngine.XR;

public interface IPlayerBase
{
}

public abstract class PlayerBase : MonoBehaviour, IPlayerBase
{
    #region user places

    private const float UpperPadding = 1.5f, BottomPadding = 1f;

    private static readonly Vector2[] UserPositions =
    {
        new Vector2(0, -6 + BottomPadding),
        new Vector2(0, 6 - UpperPadding),
        new Vector2(3.5f, 0),
        new Vector2(-3.5f, 0),
    };

    private static readonly Vector3[] UserRotations =
    {
        new Vector3(),
        new Vector3(0, 0, 180),
        new Vector3(0, 0, 90),
        new Vector3(0, 0, -90),
    };

    #endregion

    #region services

    [Inject] protected readonly Card.Factory _cardFactory;
    [Inject] protected readonly IRoomController _roomController;
    [Inject] protected readonly IController _controller;
    [Inject] protected readonly IGround _ground;

    #endregion

    protected const int HandSize = 4;

    protected List<Card> HandCards { get; } = new List<Card>();
    protected int Turn { get; private set; }

    protected void ThrowBase(Card card, ThrowResponse response)
    {
        _ground.Throw(card, response.EatenCardsIds);

        if (response.Basra) AddBasra();
        if (response.BigBasra) AddBigBasra();

        eatenCount += response.EatenCardsIds.Length;
        eatenText.text = eatenCount.ToString();

        HandCards.Remove(card);
        OrganizeHand();

        _roomController.NextTurn();
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
        var pointer = new Vector3(-(HandSize / 2) * Card.Bounds.x, 0, 0);
        pointer.x -= pointer.x / 2f;
        var spacing = new Vector3(Card.Bounds.x, 0, .05f);

        if (this is IPlayer)
            HandCards.ForEach(card => card.transform.eulerAngles = Vector3.up * 180);

        foreach (var card in HandCards)
        {
            card.transform.DOLocalMove(pointer, .75f);
            pointer += spacing;
        }
    }


    public class Factory
    {
        private readonly IInstantiator _instantiator;
        private readonly GameObject _playerPrefab;
        private readonly GameObject _oppoPrefab;

        [Inject]
        public Factory(IInstantiator instantiator, GameObject playerPrefab, GameObject oppoPrefab)
        {
            _instantiator = instantiator;
            _playerPrefab = playerPrefab;
            _oppoPrefab = oppoPrefab;
        }

        public PlayerBase Create(PlayerType playerType, int turn)
        {
            var player = _instantiator.InstantiatePrefab(playerType == PlayerType.Me ? _playerPrefab : _oppoPrefab)
                .GetComponent<PlayerBase>();

            player.Turn = turn;
            player.transform.position = UserPositions[turn];
            player.transform.eulerAngles = UserRotations[turn];

            return player;
        }
    }
}