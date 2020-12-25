using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;//for push notifications

public class RxLearning : MonoBehaviour
{
    void Start()
    {
        var observable = Observable.FromCoroutine<int>(obs => Countdown(10, obs))
            .Subscribe
            (
                i => Debug.Log(i),
                e => throw e,
                () => Debug.Log("complete")
            );
        //partially understood

    }

    private IEnumerator Countdown(int amount, System.IObserver<int> observer)
    {
        if (amount < 0)
            observer.OnError(new System.ArgumentOutOfRangeException()); //exc for the observer to handle

        for (int i = amount; i == 0; i--)
        {
            observer.OnNext(i);
            yield return new WaitForSeconds(1);
        }

        observer.OnCompleted();
    }

    private IEnumerator ObservableToYield()
    {
        var obs = Observable.Timer(System.TimeSpan.FromSeconds(1f));
        yield return obs.ToYieldInstruction();
        //you can do what you need here, as an on next
        //this somehow similar to async/await vs callbacks

        var obs2 = transform.ObserveEveryValueChanged(t => t.position)
            .FirstOrDefault(p => p.z == 15f);
        yield return obs2.ToYieldInstruction();

        //UnityWebRequest
        //yield return UnityWeb
        //ObservableWWW.Get("https://www.tuxul.com").ToYieldInstruction();
    }

}
