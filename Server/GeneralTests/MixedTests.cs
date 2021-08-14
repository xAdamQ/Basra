using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace GeneralTests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test3()
        {
            var a1 = new a(_testOutputHelper);
            var a2 = new a(_testOutputHelper);
            Action ac1 = a1.tstFun;
            Action ac12 = a1.tstFun;
            Action ac2 = a2.tstFun;

            Action ac3 = () => a1.tstFun();
            Action ac32 = () => a1.tstFun();
            Action ac4 = () => a2.tstFun();

            Assert.NotEqual(ac1, ac2);
            Assert.NotEqual(ac3, ac4);
            Assert.Equal(ac12, ac1);
            Assert.NotEqual(ac32, ac3);
        }

        [Fact]
        public async Task Test4()
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(1000);

            await Task.Delay(2000, source.Token).ContinueWith(_ =>
            {
                if (!_.IsCanceled) _testOutputHelper.WriteLine("done 9999999999999999");
            });

            // await Task.Delay(2000, source.Token).ContinueWith(t =>
            // {
            //     _testOutputHelper.WriteLine("done");
            //     _testOutputHelper.WriteLine($"task state is {t.Status}");
            // });
        }

        [Fact]
        public void Test5()
        {
            var arr = new List<int>() { 0, 5, 6 };
            _testOutputHelper.WriteLine(arr.IndexOf(6).ToString());
        }

        [Fact]
        public void Test6()
        {
            var a = 17;
            var b = 5;

            b = a += 3;
            // a += b = 3;
            _testOutputHelper.WriteLine($"{a},,,{b}");
        }

        public class a
        {
            private readonly ITestOutputHelper _testOutputHelper;
            public a(ITestOutputHelper testOutputHelper)
            {
                _testOutputHelper = testOutputHelper;
            }
            public void tstFun()
            {
                _testOutputHelper.WriteLine("tst fun called");
            }
        }


        public static int ConvertTurnToPlayerIndex(int turn, int myTurn, int roomCapacity)
        {
            if (turn == myTurn) return 0;

            var newTurn = myTurn;
            for (var playerIndex = 1; playerIndex < roomCapacity; playerIndex++)
            {
                newTurn = ++newTurn % roomCapacity;
                if (newTurn == turn) return playerIndex;
            }

            throw new System.Exception("couldn't convert");
        }

        [Theory]
        [InlineData(2, 1, 3, 1)]
        [InlineData(1, 1, 3, 0)]
        [InlineData(0, 1, 3, 2)]
        [InlineData(0, 2, 3, 1)]
        [InlineData(1, 2, 3, 2)]
        [InlineData(2, 2, 3, 0)]
        public void Test7(int turn, int myTurn, int capacity, int playerIndex)
        {
            var result = ConvertTurnToPlayerIndex(turn, myTurn, capacity);
            Assert.Equal(playerIndex, result);
        }


        [Fact]
        public void Test8()
        {
            var x = new x() { prop1 = 1 };
            var zx = (z)x;
            zx.prop2 = 11;
            zx.prop1 = 13;

            _testOutputHelper.WriteLine(zx.prop2.ToString());
            _testOutputHelper.WriteLine(zx.prop1.ToString());
        }


        [Fact]
        public void Test9()
        {
            var bag = new ConcurrentBag<string>();

            bag.Add(null);
            bag.Add("1");
            bag.Add("2");
            bag.Add(null);
            bag.Add("4");
            bag.Add(null);

            for (int i = 0; i < 8; i++)
            {
                if (bag.IsEmpty)
                {
                    _testOutputHelper.WriteLine("it was empty");
                    continue;
                }

                bag.TryTake(out string res);
                _testOutputHelper.WriteLine(res ?? "it was null");
            }
        }

        [Fact]
        public void Test10()
        {
            var obj = new r();
            obj.tstDic = new Dictionary<TstEnum, string>()
            {
                {TstEnum.val1, "hello"},
                {TstEnum.val2, "there"},
                {TstEnum.val3, "you"},
            };

            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(obj));
        }

        [Fact]
        public void Test11()
        {
            var obj = new Mock<ID>().Object;
            obj.TstEvent += () => { _testOutputHelper.WriteLine("hello"); };
        }

        [Fact]
        public void BuildString()
        {

        }
    }

    class x
    {
        public int prop1 { get; set; }
    }

    class z : x
    {
        public int prop2 { get; set; }
    }

    public enum TstEnum
    {
        val1,
        val2,
        val3,
    }

    public class r
    {
        public Dictionary<TstEnum, string> tstDic { get; set; }
    }

    public interface ID
    {
        event Action TstEvent;
    }

    public class D : ID
    {
        public event Action TstEvent;
    }
}