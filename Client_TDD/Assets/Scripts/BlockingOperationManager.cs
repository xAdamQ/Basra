using System;
using Cysharp.Threading.Tasks;
using Zenject;

public class BlockingOperationManager
{
    // private readonly IBlockingPanel _blockingPanel;

    public static BlockingOperationManager I { get; private set; }

    [Inject]
    public BlockingOperationManager()
    {
        // _blockingPanel = blockingPanel;
        I = this;
    }

    /// <summary>
    /// invoke, block, and forget
    /// </summary>
    public void Forget(UniTask operation, Action onComplete = null)
    {
        Start(operation).Forget(e => throw e);
    }

    /// <summary>
    /// uses BlockingPanel 
    /// </summary>
    public async UniTask Start(UniTask operation)
    {
        BlockingPanel.I.Show();
        try
        {
            await operation;
            BlockingPanel.I.Hide();
        }
        catch (BadUserInputException) //todo test if you can get bad user input exc here
        {
            BlockingPanel.I.Hide("operation is not allowed");
            throw;
        }
    }

    public void Forget<T>(UniTask<T> operation, Action<T> onComplete)
    {
        Start(operation).ContinueWith(onComplete).Forget(e => throw e); //the error exception happens normally inside start
    }

    /// <summary>
    /// uses BlockingPanel 
    /// </summary>
    public async UniTask<T> Start<T>(UniTask<T> operation)
    {
        BlockingPanel.I.Show();
        try
        {
            var result = await operation;
            BlockingPanel.I.Hide();
            return result;
        }
        catch (BadUserInputException) //todo test if you can get bad user input exc here
        {
            BlockingPanel.I.Hide("operation is not allowed");
            throw;
        }
    }
}