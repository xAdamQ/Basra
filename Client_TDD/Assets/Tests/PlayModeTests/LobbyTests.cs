using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayModeTests
{
    public class LobbyTests
    {
        [Test]
        public void LobbyTestSimplePasses()
        {
            // Use the Assert class to test conditions.
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator RequestRoom_ShouldEnter()
        {
            // var lobby = new Lobby();
            // // var lobby = new GameObject().AddComponent<Lobby>();
            // lobby.RequestRoom(2, 1);

            // Use the Assert class to test conditions.
            // yield to skip a frame
            yield return null;
        }
    }
}