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
            Debug.Log("hola");
            yield return null;
        }
    }
}