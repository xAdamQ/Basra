using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Basra.Client.Test
{
    public class TiimerTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TiimerTestsSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TiimerTestsWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }

        //[UnityTest]
        //public IEnumerator TestingTimer_ShouldCallTicking()
        //{
        //    var timer = new UniTaskTimer(9000, 500);
        //    timer.Ticking += () =>
        //    {
        //        Debug.Log("Hello");
        //        MonoBehaviour.print("HelloMono");
        //    };

        //    var startTask = timer.Start();
        //    //yield return startTask.
        //    yield return new WaitUntil(() => startTask.IsCompleted);
        //    Debug.Log("After");
        //    //yield return new WaitForSeconds(12);

        //}
        [UnityTest]
        public IEnumerator TestingTimer_ShouldCallTicking() => UniTask.ToCoroutine(async () =>
         {
             var timer = new UniTaskTimer(9000, 500);
             timer.Ticking += (Progress) =>
             {
                 Debug.Log("Hello: " + Progress);
                 MonoBehaviour.print("HelloMono");
             };

             await timer.Play();

             Debug.Log("After");
         });
        [UnityTest]
        public IEnumerator TestingTimer_ShouldStop() => UniTask.ToCoroutine(async () =>
         {
             var timer = new UniTaskTimer(9000, 500);
             timer.Ticking += (Progress) =>
             {
                 Debug.Log("Hello: " + Progress);
                 MonoBehaviour.print("HelloMono");
             };

             var tasks = new UniTask[]
             {
                timer.Play(),
                UniTask.Delay(4000).ContinueWith(()=>timer.Stop())
             };

             await UniTask.WhenAll(tasks);

             Debug.Log("After");
         });
    }
}
