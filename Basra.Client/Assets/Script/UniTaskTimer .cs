using Cysharp.Threading.Tasks;
using System;

namespace Basra.Client
{
    public class UniTaskTimer : ITimer
    {
        //in milliseconds
        private int _timeStep;
        private int _interval;

        public event Action Elapsed;
        public event Action<float> Ticking;

        private bool stopped;

        public UniTaskTimer(int totalTime, int timeStep, Action elapsed = null, Action ticked = null)
        {
            _interval = totalTime;
            _timeStep = timeStep;
        }

        public async UniTask Play()
        {
            var remainingTime = _interval;
            var ticksCount = _interval / _timeStep;
            for (var i = 0; i < ticksCount; i++)
            {
                remainingTime -= _timeStep;

                var progress = (float)i / ticksCount;
                Ticking?.Invoke(progress);

                await UniTask.Delay(_timeStep);

                if (stopped) break;
            }

            if (stopped) return;

            Elapsed?.Invoke();
        }

        public void Stop()
        {
            stopped = true;
        }
    }
}