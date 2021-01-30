using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Basra.Server.Tests
{
    public class ExpressionTree
    {
        [Fact]
        public void Test()
        {
            Expression<Func<Data.RoomUser, object>> includeProperty = u => u.IsReady;

            Console.WriteLine("");
            //Express`
            //includeProperty.Body
        }
    }
}
