using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public interface IPlayer
{
}

public class Player : PlayerBase, IPlayer
{
    private IController _controller;

    [Inject]
    public void Construct(IController controller)
    {
        _controller = controller;
    }

    public async UniTask Throw(int cardHandIndex)
    {
        await _controller.SendAsync("Throw", cardHandIndex);
        OrganizeHand();
        throw new NotImplementedException();
    }

    public void Distribute(int[] cardIds)
    {
        OrganizeHand();
        throw new NotImplementedException();
    }
}