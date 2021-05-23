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
    public void Forget(Func<UniTask> operation)
    {
        Start(operation).Forget(e => throw e);
    }
    private async UniTask Start(Func<UniTask> operation)
    {
        _blockingPanel.Show();
        try
        {
            await operation();
            _blockingPanel.Hide();
        }
        catch (BadUserInputException e) //todo test if you can get bad user input exc here
        {
            Debug.LogError(e); //should I throw this? what's the difference between throw and debug error
            _blockingPanel.Hide("operation is not allowed");
        }
    }
}