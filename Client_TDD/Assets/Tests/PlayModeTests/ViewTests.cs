using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

namespace PlayModeTests
{
    public class ViewTests : ZenjectUnitTestFixture
    {
        [Test]
        public void ViewTestsSimplePasses()
        {
            // Use the Assert class to test conditions.
        }


        [SetUp]
        private void InstallBindings()
        {
            // Container.BindFactory<MinUserView, MinUserView.BasicFactory>().FromComponentInNewPrefab();
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator MinUserView_ShouldSetData()
        {
            // var fac = new MinUserView.BasicFactory();
            

            // Use the Assert class to test conditions.
            // yield to skip a frame
            yield return null;
        }
    }
}