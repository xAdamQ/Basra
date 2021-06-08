using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        public class a
        {
            private readonly ITestOutputHelper _testOutputHelper;
            public a(ITestOutputHelper testOutputHelper)
            {
                _testOutputHelper = testOutputHelper;
            }
            public void tstFun()
            {
                Console.WriteLine("tst fun called");
            }
        }
    }
}