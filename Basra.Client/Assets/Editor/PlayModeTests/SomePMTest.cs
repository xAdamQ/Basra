using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Moq;
using Basra.Client;

namespace Basra.Client.Test
{
    public class SomePMTest
    {
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.

        [UnityTest]
        public IEnumerator MoveUpWithUpArrow()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.

            var playerGo = new GameObject("Player");

            playerGo.AddComponent<Rigidbody2D>();

            var sr = playerGo.AddComponent<SpriteRenderer>();
            sr.sprite = Sprite.Create(new Texture2D(100, 100), new Rect(0, 0, 100, 100), Vector2.zero);

            var player = playerGo.AddComponent<Player>();
            var pi = new Mock<IPlayerInput>();
            pi.Setup(obj => obj.Vertical).Returns(.5f);
            player.PlayerInput = pi.Object;


            yield return new WaitForSeconds(10f);
        }

        [UnityTest]
        public IEnumerator OverrideThrow()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.

            //create card
            //create ground
            //create room manager
            //

            //so this means simulate throw attributes then run this
            //the problem is this whole simultion thing is simulation! 


            yield return new WaitForSeconds(10f);
        }


    }
}
