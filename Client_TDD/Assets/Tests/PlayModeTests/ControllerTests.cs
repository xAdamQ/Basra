using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

public class ControllerTests : ZenjectUnitTestFixture
{
    // A Test behaves as an ordinary method
    [Test]
    public void ControllerTestsSimplePasses()
    {
    }


    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    [TestCase("abcd")]
    [TestCase("efcgs")]
    public IEnumerator Connect_ShouldConnect(string token)
    {
        var controller = Container.Resolve<Controller>();
        controller.ConnectToServer(token);


        yield return null;
    }
}