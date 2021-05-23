using Cysharp.Threading.Tasks;
using System;

public class UniTaskTimer
{
    //in milliseconds
    private readonly int _timeStep;
    private readonly int _interval;

    public event Action Elapsed;
    public event Action<float> Ticked;

    public bool Active { get; private set; }

    public UniTaskTimer(int interval, int timeStep, Action elapsed = null, Action<float> ticked = null)
    {
        _interval = interval;
        _timeStep = timeStep;
        Elapsed += elapsed;
        Ticked += ticked;
    }

    public async UniTask Play()
    {
        Active = true;

        var ticksCount = _interval / _timeStep;
        for (var i = 0; i < ticksCount; i++)
        {
            var progress = (float) i / ticksCount;
            Ticked?.Invoke(progress);

            await UniTask.Delay(_timeStep);

            if (!Active) break;
        }

        if (Active) Elapsed?.Invoke(); //did it finish normally or terminated

        Active = false;
    }

    public void Stop()
    {
        Active = false;
    }
}