// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using System.Timers;
// using Cysharp.Threading.Tasks.Triggers;
//
// namespace Basra.Client.Room
// {
//     //room has user room info like turn id
//     //hand has 2 types.. oppo, mine
//
//     public class RoomManager : MonoBehaviour
//     {
//         //this will work because room follows "Current" pattern
//         //any static you have to reinit
//         //they are static because they're set when there's no instance of this
//
//         public Text GenreText;
//
//         // private void Awake()
//         // {
//         //     AppManager.I.RoomManager = this;
//         //     // AppManager.I.Currents.RemoveAll(c => c.GetType() == GetType());
//         //     AppManager.I.Managers.Add(this);
//         // }
//         //
//         // private void OnDestroy()
//         // {
//         //     AppManager.I.Managers.Remove(this);
//         // }
//
//         public static RoomManager I;
//
//         private void Awake()
//         {
//             I = this;
//         }
//     }
// }

