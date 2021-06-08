using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;

namespace Basra.Client.Test
{
    public class ConnectionTests
    {
        [Test]
        public void ConnectionTestsSimplePasses()
        {
            // Connection.Init(new[] { });
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator ConnectionTestsWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // yield to skip a frame
            yield return null;
        }
    }
}