using System;
using Cysharp.Threading.Tasks;

namespace Basra.Client
{
    public interface ITimer
    {
        event Action Elapsed;
        event Action<float> Ticking;
        UniTask Play();
        void Stop();
    }
}