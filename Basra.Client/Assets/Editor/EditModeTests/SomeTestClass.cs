using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Basra.Client.Test
{
    public class SomeTestClass
    {
        // A Test behaves as an ordinary method
        [Test]//like [Fact]
        public void SetDamageToHalfWith50PrecentMiti()
        {
            //TDD
            //arrange
            var expected = 5;

            //act
            var actual = DamageCalc.CalcDamage(10, .5f);

            //assert
            Assert.AreEqual(expected, actual);

            // Use the Assert class to test conditions
        }

        // A Test behaves as an ordinary method
        [Test]//like [Fact]
        public void SetDamageTo2With80PrecentMiti()
        {
            //TDD
            //arrange
            var expected = 2;

            //act
            var actual = DamageCalc.CalcDamage(10, .8f);

            //assert
            Assert.AreEqual(expected, actual);

            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator SomeTestClassWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
