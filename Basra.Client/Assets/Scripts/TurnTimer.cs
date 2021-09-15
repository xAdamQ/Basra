using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public interface ITurnTimer
{
    bool IsPlaying { get; }

    event Action Elapsed;
    event Action<float> Ticked;

    void Play();
    void Stop();
}

//you can extract more generic form of this class for monobehaviour timer (move it outside room then)  /
//I merged both together and used coroutine
public class TurnTimer : MonoBehaviour, ITurnTimer
{
    private const int HandTime = 7; //the total interval

    public event Action Elapsed;
    public event Action<float> Ticked;

    public bool IsPlaying { get; private set; }

    private Coroutine activeTimerCoroutine;

    public void Play()
    {
        if (IsPlaying) Stop();
        activeTimerCoroutine = StartCoroutine(PlayEnumerator());
    }

    private IEnumerator PlayEnumerator()
    {
        IsPlaying = true;

        var ticksCount = HandTime / Time.fixedDeltaTime;
        for (var i = 0; i < ticksCount; i++)
        {
            var progress = (float)i / ticksCount;
            Ticked?.Invoke(progress);

            yield return new WaitForFixedUpdate();
        }

        Ticked?.Invoke(1);

        //in the new design you don't reach here if cancelled
        Elapsed?.Invoke();

        IsPlaying = false;
    }

    public void Stop()
    {
        if (activeTimerCoroutine != null)
            StopCoroutine(activeTimerCoroutine);

        IsPlaying = false;
    }
}