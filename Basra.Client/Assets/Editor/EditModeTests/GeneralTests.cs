using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using UnityEngine;
using UnityEngine.TestTools;
using Basra.Client.Room;

namespace Basra.Client.Test
{
    //room testing now
    [TestFixture(Category = "mt tests")]
    public class GeneralTests
    {
        [Test]
        public void Throw()
        {
            //create new card
            //give it a mock user, room and turn timer
            //the mock room would have mock ground

            var room = new Mock<IRoomManager>();
            var user = new Mock<IUser>();
            var timer = new Mock<ITurnTimer>();
            var ground = new Mock<IGround>();

            //issue 001 can't craete monobehaviuor script to test the actual code
            var card = UnityEngine.Object.Instantiate(new GameObject());



        }
    }
}
