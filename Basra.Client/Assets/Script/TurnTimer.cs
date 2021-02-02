using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
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
    //you can extract more generic form of this class for monobehaviour timer (move it outside room then)  //timer is a progress bar
    public class TurnTimer : MonoBehaviour
    {
        [SerializeField] TextMesh RemainingTimeText;
        [SerializeField] GameObject View;

        private UniTaskTimer Timer;

        private void Awake()
        {
            Timer = new UniTaskTimer(User.HandTime, 50);
            Timer.Ticking += OnTimerTicking;
        }

        private void OnTimerTicking(float progress)
        {
            RemainingTimeText.text = (User.HandTime / 1000 * (1 - progress)).ToString("f2");
        }

        private void Start()
        {
            transform.eulerAngles = Vector3.zero;
        }

        public void AddToOnElapsed(Action action)
        {
            Timer.Elapsed += action;
        }

        public void Play()
        {
            Show();
            Timer.Play().Forget((ecx) => Debug.LogError(ecx.Message));
        }
        public void Stop()
        {
            Timer.Stop();
            Hide();
        }

        private void Hide()
        {
            View.SetActive(false);
        }
        private void Show()
        {
            View.SetActive(true);
        }
    }
}
