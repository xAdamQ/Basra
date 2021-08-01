using Basra.Common;
using Basra.Server.Tests;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Basra.Server.Services;
using Microsoft.Extensions.Configuration;
using Basra.Server.Models;

namespace Basra.Server.Tests
{
    public class MixedTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public MixedTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void SignInWithFbigToken()
        {
            var tstToken = "jeGd6FpkrrtLlMPN5-CJoW0jAjVzsSqkm1p67qWSBO4.eyJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsImlzc3VlZF9hdCI6MTYyNzc1MDM2MCwicGxheWVyX2lkIjoiMzYzMjgxNTU2MDA5NDI3MyIsInJlcXVlc3RfcGF5bG9hZCI6Im15X21ldGFkYXRhIn0";

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(m => m["Secrets:AppSecret"]).Returns("f3a20105a05d548258d8be8bd56ad2b3");

            var sm = new FbigSecurityManager(configMock.Object, new Mock<IMasterRepo>().Object, new Mock<ISessionRepo>().Object);
            sm.ValidateToken(tstToken, out var playerId);

            testOutputHelper.WriteLine(playerId.ToString());
        }

    }
}