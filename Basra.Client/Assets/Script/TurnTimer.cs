using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Basra.Client.Room
{
    //you can extract more generic form of this class for monobehaviour timer (move it outside room then)  //timer is a progress bar
    public class TurnTimer : MonoBehaviour, ITurnTimer
    {
        [SerializeField] TextMesh RemainingTimeText;
        [SerializeField] GameObject View;

        private static float TimeStep = 0.1f;
        public event System.Action Elapsed;
        private Coroutine TimerCoroutine;

        private void Start()
        {
            MakeRotaionReadable();
        }

        private void MakeRotaionReadable()
        {
            transform.eulerAngles = Vector3.zero;
        }

        public void Play()
        {
            IEnumerator coroutine()
            {
                Show();

                var remainingTime = Room.User.HandTime;
                var framCount = Room.User.HandTime / TimeStep;
                for (var i = 0; i < framCount; i++)
                {
                    RemainingTimeText.text = remainingTime.ToString("f2");
                    remainingTime -= TimeStep;
                    yield return new WaitForSeconds(TimeStep);
                }

                Elapsed?.Invoke();
            }

            TimerCoroutine = StartCoroutine(coroutine());
        }

        public void Stop()
        {
            StopCoroutine(TimerCoroutine);
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
