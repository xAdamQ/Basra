using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Zenject;


public enum CardOwner
{
    Me,
    Oppo,
    Ground
}

public class Card : MonoBehaviour
{
    [Inject] private readonly Front.Factory _frontFactory;
    [Inject] private readonly IGround _ground;

    public IPlayerBase Player;
    public static Vector2 Bounds = new Vector2(.75f, 1f);
    public Front Front { get; set; }

    public void AddFront(int id)
    {
        Front = _frontFactory.Create(id, transform);
    }

    public class Factory
    {
        private readonly IInstantiator _instantiator;
        private readonly GameObject _prefab;
        public Factory(IInstantiator instantiator, GameObject prefab)
        {
            _instantiator = instantiator;
            _prefab = prefab;
        }

        private Card Create(Transform parent, CardOwner cardOwner, int frontId)
        {
            var card = _instantiator.InstantiatePrefab(_prefab, parent).GetComponent<Card>();

            //init
            if (frontId != -1)
                card.AddFront(frontId);

            return card;
        }

        public Card CreateGroundCard(int frontId, Transform parent)
        {
            return Create(null, CardOwner.Ground, frontId);
        }
        public Card CreateMyPlayerCard(int frontId, Transform parent)
        {
            return Create(parent, CardOwner.Me, frontId);
        }
        public Card CreateOppoCard(Transform parent)
        {
            return Create(parent, CardOwner.Oppo, -1);
        }
    }

    public void OnMouseDown()
    {
        if (Player != null && Player is IPlayer player && player.IsPlayable)
            StartCoroutine(Drag(transform.position));
    }

    private IEnumerator Drag(Vector3 initialPoz)
    {
        var player = (Player as IPlayer);
        while (Input.GetMouseButton(0) && player.IsPlayable)
        {
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            yield return null;
        }

        if (player.IsPlayable &&
            transform.position.x < _ground.TopRightBound.x &&
            transform.position.y < _ground.TopRightBound.y &&
            transform.position.x > _ground.LeftBottomBound.x &&
            transform.position.y > _ground.LeftBottomBound.y)
        {
            player.Throw(this);
        }
        else
        {
            transform.position = initialPoz;
        }
    }
}