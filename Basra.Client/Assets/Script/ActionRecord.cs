using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Basra.Client
{
    public class ActionRecord
    {
        private MonoBehaviour Initiator;
        private string Verb;
        private object[] ServerArgs;

        public ActionRecord(MonoBehaviour initiator, string verb, params object[] serverArgs)
        {
            Initiator = initiator;
            Verb = verb;
            ServerArgs = serverArgs;
        }

        private void InternalCall()
        {
            var methodInfo = Initiator.GetType().GetMethod("Internal" + Verb);
            methodInfo.Invoke(Initiator, null);
        }
        // private void ServerCall()
        // {
        //     RevertTransforms();
        //     var methodInfo = Initiator.GetType().GetMethod("Server" + Verb);
        //     methodInfo.Invoke(Initiator, null);
        // }

        private Dictionary<MonoBehaviour, TransformValue> Transforms = new Dictionary<MonoBehaviour, TransformValue>();
        public void RecoredTransform(MonoBehaviour monoBehaviour)
        {
            Transforms.Add(monoBehaviour, new TransformValue(monoBehaviour.transform));
        }

        public void RemoteCall()
        {
            AppManager.I.HubConnection.Send(Verb, ServerArgs)
            .OnComplete(futures =>
            {
                Debug.Log("compleeeeeeeeeeeeeeeeeeeeeeeeeeted call");
                InternalCall();
                // LastAction.
                //move to refelctions because it will save alot
                // typeof(LobbyManger).GetMethod(nameof(Lobby.))
            })
            .OnError(exc =>
            {
                Debug.Log("eroooooooooooooooooooooooooooooooooor call");
                // ServerCall();
                //I choose the user exceptions, so you can make some uses with it
                //reverse target and correct the action
            });
        }

        private void RevertTransforms()
        {
            foreach (var item in Transforms)
            {
                item.Value.LoadTo(item.Key);
            }
        }
    }

    public struct TransformValue
    {
        public Vector3 Position, EularAngles, Scale;

        public TransformValue(UnityEngine.Transform transform)
        {
            Position = transform.position;
            EularAngles = transform.eulerAngles;
            Scale = transform.localScale;
        }

        public void LoadTo(MonoBehaviour monoBehaviour)
        {
            monoBehaviour.transform.position = Position;
            monoBehaviour.transform.eulerAngles = EularAngles;
            monoBehaviour.transform.localScale = Scale;
        }
    }
}