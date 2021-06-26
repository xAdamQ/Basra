using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class BlockingOperationManager
{
    private readonly IBlockingPanel _blockingPanel;

    [Inject]
    public BlockingOperationManager(IBlockingPanel blockingPanel)
    {
        _blockingPanel = blockingPanel;
    }

    /// <summary>
    /// invoke, block, and forget
    /// </summary>
    public void Forget(UniTask operation, Action onComplete = null)
    {
        Start(operation).Forget(e => throw e);
    }
    public async UniTask Start(UniTask operation)
    {
        _blockingPanel.Show();
        try
        {
            await operation;
            _blockingPanel.Hide();
        }
        catch (BadUserInputException) //todo test if you can get bad user input exc here
        {
            _blockingPanel.Hide("operation is not allowed");
            throw;
        }
    }

    public void Forget<T>(UniTask<T> operation, Action<T> onComplete)
    {
        Start(operation).ContinueWith(onComplete).Forget(e => throw e); //the error exception happens normally inside start
    }
    public async UniTask<T> Start<T>(UniTask<T> operation)
    {
        _blockingPanel.Show();
        try
        {
            var result = await operation;
            _blockingPanel.Hide();
            return result;
        }
        catch (BadUserInputException) //todo test if you can get bad user input exc here
        {
            _blockingPanel.Hide("operation is not allowed");
            throw;
        }
    }
}