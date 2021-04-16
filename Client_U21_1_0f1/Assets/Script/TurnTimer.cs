using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Zenject;

/*

design your classes with best practices i.e loosely coupled with interfaces
use an instance of it in the humble object
then you can test your logic class

in the article he used the humble as injector for the logic
so the monobehavioural implementation is inside the monobehaviuor
buttt, this way you can't test this!

*/

namespace Basra.Client.Room
{
    public interface ITurnTimerInterface
    {
        void Show();
        void Hide();
        void UpdateRemainingTimeText(float progress);
    }

    public class TurnTimerInterface : MonoBehaviour, ITurnTimerInterface
    {
        [SerializeField] TextMesh RemainingTimeText;
        [SerializeField] GameObject View;

        private void Start()
        {
            transform.eulerAngles = Vector3.zero;
        }

        public void Hide()
        {
            View.SetActive(false);
        }

        public void Show()
        {
            View.SetActive(true);
        }

        public void UpdateRemainingTimeText(float progress)
        {
            RemainingTimeText.text = (User.HandTime / 1000 * (1 - progress)).ToString("f2");
        }
    }

    //you can extract more generic form of this class for monobehaviour timer (move it outside room then)  //timer is a progress bar
    public class TurnTimer
    {
        public ITurnTimerInterface Interface { get; }

        private UniTaskTimer Timer;

        [Inject]
        public TurnTimer(ITurnTimerInterface turnTimerInterface)
        {
            Interface = turnTimerInterface;
            Timer = new UniTaskTimer(User.HandTime, 50);
            Timer.Ticking += OnTimerTicking;
        }

        private void OnTimerTicking(float progress)
        {
            Interface.UpdateRemainingTimeText(progress);
        }

        public void AddToOnElapsed(Action action)
        {
            Timer.Elapsed += action;
        }

        public void RemoveFromOnElapsed(Action action)
        {
            Timer.Elapsed -= action;
        }

        public void Play()
        {
            Interface.Show();
            Timer.Play().Forget((ecx) => Debug.LogError(ecx.Message));
        }

        public void Stop()
        {
            Timer.Stop();
            Interface.Hide();
        }

        public class Factory : PlaceholderFactory<TurnTimer>
        {
        }
    }
}