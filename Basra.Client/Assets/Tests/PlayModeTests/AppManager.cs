using System.Collections;
using UnityEngine;
using NUnit.Framework;
using Moq;
using UnityEngine.TestTools;

namespace Basra.Client.Tests
{
    public class AppManager : MonoBehaviour
    {
        [UnityTest]
        public IEnumerator Connect()
        {
            var am = new Client.AppManager();

            am.Connect("12");

            yield return null;
        }
    }
}