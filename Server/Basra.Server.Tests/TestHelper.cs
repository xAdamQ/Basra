using System.Reflection;
using System.Threading.Tasks;
using Basra.Server.Extensions;

namespace Basra.Server.Tests
{
    public static class TestHelper
    {
        public static async Task<object> CallAsyncPrivateMethod(string methodName, object instance, object[] args)
        {
            var mi = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);

            return await mi.InvokeAsync(instance, args);
        }

        public static async Task CallAsyncPrivateAction(string methodName, object instance, object[] args)
        {
            var mi = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);

            await mi.InvokeActionAsync(instance, args);
        }

        public static object CallPrivateMethod(string methodName, object instance, object[] args)
        {
            var mi = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);

            return mi.Invoke(instance, args);
        }
    }
}