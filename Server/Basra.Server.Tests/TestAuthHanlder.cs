using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Basra.Server.Data;
using Basra.Server.Exceptions;
using Basra.Server.Services;
using Castle.Components.DictionaryAdapter;
using Hangfire;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Basra.Server.Extensions;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.Options;

namespace Basra.Server.Tests
{
    public class TestAuthHanlder
    {
        private readonly ITestOutputHelper testOutputHelper;

        public TestAuthHanlder(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task TestGetToken()
        {
            //Given
            var authCode =
                "DgGAs2yHFMN8Ey9K+YbhmF6z3IKJwmfWxp9ogDYcgGGuVEwhDD5EoZkhfV0CNJz1I3bz72VF0bDaraC/eiKGIoc2RQJxrTpqFDgJTM092Q6aZzzTUXQvfmU4fuXEWUYzd9etMK46JnQPZvB4pEsDR0ffV89GTwxjO/lVuwgB0OYhKg2M4umR+nSgCun7VmslYx7vCstLZiVhL8QHW2piIuVv/WzlFpVBuC3M5VU7g+EmChFVsZiZarvYMfH4b4JDB4AabXUfuzY=";

            //When
            var res = await MasterAuthenticationHandler.GetTokenByHuaweiAuthCode(authCode);

            //Then
            testOutputHelper.WriteLine("token is::  " + res);
            Assert.NotNull(res);
        }

        [Fact]
        public async Task TestGetUserData()
        {
            //Given
            var token =
                "CwGAs2yHQR8gzYqlsW7VQP0P7denAl6vYX748BSac6/tWc/Cgq78xo5hnMQ+v2bq+W5ReWc0B7xkC9JzHZWycz/KvJVw6WIr72W7sYz774YD0jE=";

            //When
            var res = await MasterAuthenticationHandler.GetUserDatByToken(token);

            //Then
            Assert.Equal("xad***@***il.com", res.name);
            Assert.NotNull(res.picUrl);

            testOutputHelper.WriteLine(res.picUrl);
        }

        [Fact]
        public void SignVer()
        {
            var pubKey =
                "MIIBojANBgkqhkiG9w0BAQEFAAOCAY8AMIIBigKCAYEAgeC4JKFoBKy94Ou2bupJPHw5IFFPaiqdfmpg8KjjrpkvCCEu7m4Xp5V82h22KML5XkntChMBkDQUCdyb3XLvzKGICJLb9/8tmimZ/OZb7N+jz09wJ5uIdoDrLJl14yHb6W27T0WirVAcjBwDbsx43d/YZPvOcsFXtQzFgKxsrNYSggZs+CmDTktvH72+142i9nAwd+Gt6IbTy9+gpKTyi+PEoql9xodOsr7N4xNrbq2XHFIRcbH3HmWlgYPE6mzqc3tqKmuqP1KJuQ6sPI96mecIvYqQjc7eop0YNW8Ksej+Wy1yHtdSt7VsjqJIrnd0WxDf5g4IZ4Zsz3s2kI4hRoqKAWWZKlqECWeJxMLN/uNRwyZqH7LEP/ZIqn+cqsqPW4BbHs7I+F8AXD98ggTx89SiI9UHw7axgH5Z+zbR0ZI6DSL6mXTLqQfiqa1q5F6moExyL7cGQA6tVyISh6AFKWjJwqRwzRhOzCT6Il1LJYQ3YRc0wT9OrPBpVoTyvU/LAgMBAAE=";
            var sign =
                "bAXvqF3zHOFJN9TRPRiKk6kbcKPcjm5I5DId8sNnSMQlUqpdXGzBQbLFjAbTEaMabrPa1suZaWFVUpu/2lCwPM6T5UzcVVn2nMgtEwV2vUlBoyO5feRplZhH5jY4iArbIzdph8PXWgUCZ6qE5Fv/BQhOFavGHg+hUcGDh50JSO2+jPYuPeAQ0R2+uzBwbChr3U/N9GYGCXtKlIaxtggIotd27usLTCDLUv+fJfQszROneE0QY9+tomlC0w1ScPqYhNC7OkfKgnLArr7FwPAD/zlCe5iKaW9Y3VOjHUqCQYTMVlJ+MipkMmgHmX0ZrTbGJ0zwA8tM207dQiUN94xdvNPMCvbC3VEnEx4mdL6pxnGnJXUc0ybZcaNCARQOcF+Z2tN4k6XGlkjfdKUWQz5KPSuoN3hBPnwLk3K4AtwA48waYPEaixwajsfXJOXN5LOGEoSR858LQ4Pc9Ax3wTzFyx8B2fYsx7yU8qkB1mZfy7Y00BCURbFA+4FPeSebBLBF";
            var content = "";

            LobbyManager.VerifyIAPSign(content, sign, pubKey);
        }


        [Fact]
        public async Task ValidateFbAccTokenShoudPass()
        {
            var config = new Mock<IConfiguration>();
            config.Setup(c => c["Secrets:FbAppToken"])
                .Returns("GG|146449481020082|6w6LKOzaov1bwO2NVC6kd-oK9lM");

            var sm = new SecurityManager(config.Object, new Mock<IMasterRepo>().Object,
                new Mock<ILogger<SecurityManager>>().Object);

            var token =
                "GGQVliU3lnMGQ4OUxueUs1T0ZAycmk3R0M2QVp1d2lsNEVMSVVkWTJzVDE3RVF3bFBISEZAOOWJHMEFqMmN3c1BKNzlpaHUyZAUJjcTVXV3lvaXRjWi1raTgxYkJKMjJEdjd0a0p4UU5zd0prbm41SjZAQVXFmeXYxclVTeXZA6c2lUbjNNMEw2ZAEVzVnlNS1lhRTI5cHpELXNWS0l0bGVVNzVJOFlKZAwZDZD";

            var res = await sm.ValidateFbAccToken(token);

            testOutputHelper.WriteLine(res.ToString());

            Assert.True(res);
        }

        [Fact]
        public async Task ValidateFbAccToken_ShoudUserInputFail()
        {
            var config = new Mock<IConfiguration>();
            config.Setup(c => c["Secrets:FbAppToken"])
                .Returns("GG|146449481020082|6w6LKOzaov1bwO2NVC6kd-oK9lM");

            var sm = new SecurityManager(config.Object, new Mock<IMasterRepo>().Object,
                new Mock<ILogger<SecurityManager>>().Object);

            var token = "some_wrong_token";

            var message = (await Assert.ThrowsAsync<BadUserInputException>(() =>
                sm.ValidateFbAccToken(token))).Message;

            testOutputHelper.WriteLine(message);
        }

        [Fact]
        public async Task ValidateFbAccToken_ShoudMyServerFail()
        {
            var config = new Mock<IConfiguration>();
            config.Setup(c => c["Secrets:FbAppToken"])
                .Returns("some_wrong_app_token");

            var sm = new SecurityManager(config.Object, new Mock<IMasterRepo>().Object,
                new Mock<ILogger<SecurityManager>>().Object);

            var token =
                "GGQVliU3lnMGQ4OUxueUs1T0ZAycmk3R0M2QVp1d2lsNEVMSVVkWTJzVDE3RVF3bFBISEZAOOWJHMEFqMmN3c1BKNzlpaHUyZAUJjcTVXV3lvaXRjWi1raTgxYkJKMjJEdjd0a0p4UU5zd0prbm41SjZAQVXFmeXYxclVTeXZA6c2lUbjNNMEw2ZAEVzVnlNS1lhRTI5cHpELXNWS0l0bGVVNzVJOFlKZAwZDZD";

            var message = (await Assert.ThrowsAsync<SecurityManager.FbApiError>(() =>
                sm.ValidateFbAccToken(token))).Message;

            testOutputHelper.WriteLine(message);
        }

        [Fact]
        public async Task TestGetFbProfile_ShouldWork()
        {
            var sm = new SecurityManager(new Mock<IConfiguration>().Object,
                new Mock<IMasterRepo>().Object, new Mock<ILogger<SecurityManager>>().Object);

            var token =
                "GGQVliU3lnMGQ4OUxueUs1T0ZAycmk3R0M2QVp1d2lsNEVMSVVkWTJzVDE3RVF3bFBISEZAOOWJHMEFqMmN3c1BKNzlpaHUyZAUJjcTVXV3lvaXRjWi1raTgxYkJKMjJEdjd0a0p4UU5zd0prbm41SjZAQVXFmeXYxclVTeXZA6c2lUbjNNMEw2ZAEVzVnlNS1lhRTI5cHpELXNWS0l0bGVVNzVJOFlKZAwZDZD";

            var profile = await sm.GetFbProfile(token);

            testOutputHelper.WriteLine(profile.ToString());

            Assert.Equal("MahmoudXV", profile.name);
            Assert.Equal("1687355904791192", profile.id);
            // Assert.Equal(
            // "https://platform-lookaside.fbsbx.com/platform/profilepic/?asid=1687355904791192&gaming_photo_type=unified_picture&ext=1635379893&hash=AeRfXSsYftxX_nUbaTE",
            // profile.picUrl);
        }
    }
}