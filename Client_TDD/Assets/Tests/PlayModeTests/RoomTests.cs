using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Moq;
using Zenject;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;

[TestFixture]
public class RoomTests : ZenjectUnitTestFixture
{
    [Test]
    public void RunTest1()
    {
    }

    [UnityTest]
    public IEnumerator Player_Distribute()
    {
        Player player = null;

        var controller = new Mock<IController>();
        controller.Setup(_ => _.SendAsync(It.IsAny<string>()))
            .Returns<string, object[]>((method, args) =>
            {
                Debug.Log(
                    $"the method {method} should be called on the server, " +
                    $"with args {string.Join(", ", args)}");
                return UniTask.FromResult(new object());
            });

        // StaticContext.Container.BindFactory<>()
            
        StaticContext.Container.BindInstance(controller.Object);
        
        
        player.Distribute(new[] {1, 2, 3, 4});

        yield return null;
    }
}