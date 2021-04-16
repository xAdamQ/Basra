using UnityEngine;

//global object disabling
//every onmousedown is checked first

//I want to make it real instant
//so it should simulate adding a card
//so I have to add custom revert method!

namespace Basra.Client
{
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