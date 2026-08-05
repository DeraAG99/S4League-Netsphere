using System.Threading.Tasks;
using BlubLib.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace System.Diagnostics
{
    public static class ProcessExtensions
    {
        public static Task WaitForExitAsync(this Process @this)
        {
            @this.EnableRaisingEvents = true;

            var tcs = new TaskCompletionSource();
            @this.Exited += (s, e) => tcs.TrySetResult();
            return tcs.Task;
        }
    }
}
