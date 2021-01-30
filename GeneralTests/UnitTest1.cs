using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Expression<Func<RoomUser, object>> includeProperty = u => u.IsReady;
            //Expression<Func<RoomUser, object>> includeProperty2 = u => nameof(u.IsReady);
            //Expression
            //var memExp = includeProperty.Body as MemberExpression;

            //var memExp2 = ((MemberExpression)((UnaryExpression)includeProperty.Body).Operand).Member.Name;
            //Debug.WriteLine(memExp.Member.Name);
            //Debug.WriteLine(includeProperty.Body.ToString());

            var unaryExpo = includeProperty.Body as UnaryExpression;
            var operand = unaryExpo.Operand;
            var memExp3 = operand as MemberExpression;
            var name = memExp3.Member.Name;


            //includeProperty.Body
        }

        [Fact]
        public void Test2()
        {
            //var a = new DbContext.RoomUsers.Where(u => u.UserId == userId).Select(u => u.IsReady)

        }


    }

    public class RoomUser
    {
        public List<int> Cards { get; set; }
        public const int HandTime = 11;

        public int Score { get; set; }
        public int BasraCount { get; set; }
        public int BigBasraCount { get; set; }
        public int EatenCardsCount { get; set; }

        public bool IsReady { get; set; }
    }
}
