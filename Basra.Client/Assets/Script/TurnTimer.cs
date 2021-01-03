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

client.room.component
from comp user access normal user: room.User
from comp user access normal room: room.roommanager
from comp user access comp room: roommanager
from comp user access comp appamanger(client): room.roommanager

client.component.room
normal user: client.room
normal appamanger: client.Appmanager
comp appmanager: 

prev ns
next ns
different ns

in the first the component types in different directories differes
second: the same non component requires start from components

you point at logical only from comp
comp has to access logical easily

*/

namespace Basra.Client.Room.Components
{
    //you can extract more generic form of this class for monobehaviour timer (move it outside room then)  //timer is a progress bar
    public class TurnTimer : MonoBehaviour
    {
        [SerializeField] TextMesh RemainingTimeText;
        [SerializeField] GameObject View;

        private UniTaskTimer Timer;

        public void Construct(UniTaskTimer Timer)
        {
            Timer.Ticking += OnTimerTicking;
        }

        private void OnTimerTicking(float progress)
        {
            RemainingTimeText.text = (Client.Room.User.HandTime / 1000 * (1 - progress)).ToString("f2");
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
        }
        public void Stop()
        {
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
