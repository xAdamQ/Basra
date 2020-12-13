using System.Collections.Generic;
using UnityEngine;
using System;

namespace Basra.Client
{
    //### can non monobehaviours use oop freely?
    //when function is called rpc, mine or serevr'???

    public class InstantRpcRecord
    {
        /// <summary>
        /// must be null when assigned, as well as any 'current'
        /// </summary>
        public static InstantRpcRecord Current;

        private Action Confirmation;

        public InstantRpcRecord(System.Action confirmation)
        {
            if (Current != null) throw new System.Exception("you are breaking the current pattern");
            Current = this;

            Confirmation = confirmation;
        }

        public void RevertVisuals()
        {
            RevertTransforms();
            Current = null;
        }
        public void Confirm()
        {
            Confirmation?.Invoke();
            Current = null;
        }

        private Dictionary<MonoBehaviour, TransformValue> Transforms = new Dictionary<MonoBehaviour, TransformValue>();
        public void RecoredTransform(MonoBehaviour monoBehaviour)
        {
            Transforms.Add(monoBehaviour, new TransformValue(monoBehaviour.transform));
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

        public TransformValue(Transform transform)
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