using System;

namespace Basra.Client.Room
{
    public interface ITurnTimer
    {
        event Action Elapsed;

        void Play();
        void Stop();
    }
}