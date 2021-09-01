using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;


public enum CardOwner
{
    Me,
    Oppo,
    Ground
}

public class Card : MonoBehaviour
{
    public IPlayerBase Player;
    public static Vector2 Bounds = new Vector2(.75f, 1f);
    public Front Front { get; set; }

    public static int RotBound = 5;

    public async UniTask AddFront(int id)
    {
        Front = await Front.Create(id, transform);
    }

    #region factory methods

    private static async UniTask<Card> Create(Transform parent, Sprite backSprite, int frontId)
    {
        var card = (await Addressables.InstantiateAsync("card", parent)).GetComponent<Card>();

        card.transform.localScale = Vector3.zero;

        //init  
        if (frontId != -1)
            await card.AddFront(frontId);

        card.GetComponent<SpriteRenderer>().sprite = backSprite;

        return card;
    }
    public static async UniTask<Card> CreateGroundCard(int frontId, Transform parent)
    {
        return await Create(parent, null, frontId);
    }
    public static async UniTask<Card> CreateMyPlayerCard(int frontId, Sprite backSprite,
        Transform parent)
    {
        return await Create(parent, backSprite, frontId);
    }
    public static async UniTask<Card> CreateOppoCard(Sprite backSprite, Transform parent)
    {
        return await Create(parent, backSprite, -1);
    }

    #endregion

    public void OnMouseDown()
    {
        if (Player != null && Player is IPlayer player && player.IsPlayable)
            StartCoroutine(Drag(transform.position));
    }

    private IEnumerator Drag(Vector3 initialPoz)
    {
        var player = (Player as IPlayer);
        while (Input.GetMouseButton(0) && player!.IsPlayable)
        {
            transform.position =
                Camera.main!.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y, 10));
            yield return null;
        }

        if (player!.IsPlayable &&
            transform.position.x < Ground.I.TopRightBound.x &&
            transform.position.y < Ground.I.TopRightBound.y &&
            transform.position.x > Ground.I.LeftBottomBound.x &&
            transform.position.y > Ground.I.LeftBottomBound.y)
        {
            player.Throw(this);
        }
        else
        {
            transform.position = initialPoz;
        }
    }
}