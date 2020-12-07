using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Threading.Tasks;

namespace Basra.Client
{
    //can you make concurrent actions?
    //yes!! you can play a card and send a message without serevr confirmation

    //### can non monobehaviours use oop freely?

    //when function is called rpc, mine or serevr'???

    public class InstantRpcRecord
    {
        private MonoBehaviour Initiator;
        private string Verb;
        private object[] ServerArgs;

        /// <summary>
        /// must be null when assigned, as well as any 'current'
        /// </summary>
        public static InstantRpcRecord Current;

        public InstantRpcRecord(MonoBehaviour initiator, string verb, params object[] serverArgs)
        {
            if (Current != null) throw new System.Exception("you are breaking the current pattern");
            Current = this;

            Initiator = initiator;
            Verb = verb;
            ServerArgs = serverArgs;
        }

        private Dictionary<MonoBehaviour, TransformValue> Transforms = new Dictionary<MonoBehaviour, TransformValue>();
        public void RecoredTransform(MonoBehaviour monoBehaviour)
        {
            Transforms.Add(monoBehaviour, new TransformValue(monoBehaviour.transform));
        }

        public void Call()
        {
            // var result = await AppManager.I.HubConnection.SendAsync(Verb, ServerArgs);
            //awaited code works well using coro

            AppManager.I.HubConnection.Send(Verb, ServerArgs)
            .OnComplete(futures =>
            {
                Debug.Log("compleeeeeeeeeeeeeeeeeeeeeeeeeeted call");
                ////### consider dispose
                Current = null;
                // LastAction.
                //move to refelctions because it will save alot
                // typeof(LobbyManger).GetMethod(nameof(Lobby.))
            })
            .OnSuccess(future =>
            {
                Debug.Log("Succcccccccceeeeeeeeeeeedeeeeeeeed call");
                ExcuteReal();
            })
            .OnError(exc =>
            {
                Debug.Log("eroooooooooooooooooooooooooooooooooor call" + exc);
                //the server call do the revert, no action needed late action with is the only possible error right now
                // ServerCall();
                //I choose the user exceptions, so you can make some uses with it
                //reverse target and correct the action
            });

        }

        private void ExcuteReal()
        {
            var methodInfo = Initiator.GetType().GetMethod("Real" + Verb);
            methodInfo.Invoke(Initiator, null);
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