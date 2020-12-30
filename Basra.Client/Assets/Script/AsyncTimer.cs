//using System;
//using System.Threading.Tasks;

//namespace Basra.Client
//{
//    public class AsyncTimer : ITimer
//    {
//        //in milliseconds
//        private int _timeStep;
//        private int _interval;

//        public event Action Elapsed;
//        public event Action Ticking;

//        private bool stopped;

//        public AsyncTimer(int totalTime, int timeStep, Action elapsed = null, Action ticked = null)
//        {
//            _interval = totalTime;
//            _timeStep = timeStep;
//        }

//        public async Task Start()
//        {
//            var remainingTime = _interval;
//            var ticksCount = _interval / _timeStep;
//            for (var i = 0; i < ticksCount; i++)
//            {
//                remainingTime -= _timeStep;

//                Ticking?.Invoke();

//                await Task.Delay(_timeStep);

//                if (stopped) break;
//            }

//            if (stopped) return;

//            Elapsed?.Invoke();
//        }

//        public void Stop()
//        {
//            stopped = true;
//        }
//    }
//}