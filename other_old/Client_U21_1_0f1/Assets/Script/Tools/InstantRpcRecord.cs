// using System.Collections.Generic;
// using UnityEngine;
// using System;

// //global object disabling
// //every onmousedown is checked first

// //I want to make it real instant
// //so it should simulate adding a card
// //so I have to add custom revert method!

// namespace Basra.Client
// {
//     //### can non monobehaviours use oop freely?
//     //when function is called rpc, mine or serevr'???

//     public class InstantRpcRecord
//     {
//         /// <summary>
//         /// must be null when assigned, as well as any 'current'
//         /// </summary>
//         public static InstantRpcRecord Current;

//         private readonly Action ConfirmationAction;
//         private readonly MonoBehaviour Initiator;

//         // private readonly Action RevertAction;

//         // public InstantRpcRecord(System.Action confirmation, System.Action revert)
//         // {
//         //     if (Current != null) throw new System.Exception("you are breaking the current pattern");
//         //     Current = this;

//         //     ConfirmationAction = confirmation;
//         //     this.RevertAction = revert;
//         // }

//         public InstantRpcRecord(System.Action confirmation, MonoBehaviour initiator)
//         {
//             if (Current != null) throw new System.Exception("you are breaking the current pattern");
//             Current = this;

//             ConfirmationAction = confirmation;
//             Initiator = initiator;
//         }

//         public void Revert()
//         {
//             RevertTransforms();
//             RevertFields();
//             // RevertAction?.Invoke();
//             Current = null;
//         }
//         public void Confirm()
//         {
//             ConfirmationAction?.Invoke();
//             Current = null;
//         }

//         private Dictionary<MonoBehaviour, TransformValue> Transforms = new Dictionary<MonoBehaviour, TransformValue>();
//         public void RecoredTransform(MonoBehaviour monoBehaviour)
//         {
//             Transforms.Add(monoBehaviour, new TransformValue(monoBehaviour.transform));
//         }
//         private void RevertTransforms()
//         {
//             foreach (var item in Transforms)
//             {
//                 item.Value.LoadTo(item.Key);
//             }
//         }

//         private Dictionary<string, object> RecordedFields = new Dictionary<string, object>();
//         public void RecordField(string name, object value)
//         {
//             RecordedFields.Add(name, value);
//         }

//         private void RevertFields()
//         {
//             var initiatorType = Initiator.GetType();
//             foreach (var kvp in RecordedFields)
//             {
//                 initiatorType.GetField(kvp.Key).SetValue(Initiator, kvp.Value);
//             }
//         }
//     }
// }