using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using Zenject;

public interface ITurnTimer
{
    bool IsPlaying { get; }

    event Action Elapsed;
    event Action<float> Ticked;

    void Play();
    void Stop();
}

//you can extract more generic form of this class for monobehaviour timer (move it outside room then)  //timer is a progress bar
//I merged both together and used coroutine
public class TurnTimer : MonoBehaviour, ITurnTimer
{
    private const int HandTime = 8; //the total interval
    private const float HandTimeStep = .1f;

    [SerializeField] private Image timerImage;

    public event Action Elapsed;
    public event Action<float> Ticked;

    public bool IsPlaying { get; private set; }

    private void Start()
    {
        Ticked += UpdateRemainingView;
    }

    private Coroutine ActiveTimerCoroutine;

    public void Play()
    {
        if (IsPlaying) Stop();
        ActiveTimerCoroutine = StartCoroutine(PlayEnumerator());
    }

    private IEnumerator PlayEnumerator()
    {
        IsPlaying = true;

        var ticksCount = HandTime / HandTimeStep;
        for (var i = 0; i < ticksCount; i++)
        {
            var progress = (float)i / ticksCount;
            Ticked?.Invoke(progress);

            yield return new WaitForSeconds(HandTimeStep);
        }

        Ticked?.Invoke(1);

        //in the new design you don't reach here if cancelled
        Elapsed?.Invoke();

        IsPlaying = false;
    }

    public void Stop()
    {
        StopCoroutine(ActiveTimerCoroutine);
        IsPlaying = false;
    }

    private void UpdateRemainingView(float progress)
    {
        timerImage.fillAmount = progress;
    }
}