using System.Collections.Concurrent;

namespace BlubLib.Collections.Concurrent
{
    public class AsyncBag<T> : AsyncCollection<T>
    {
        public AsyncBag()
            : base(new ConcurrentBag<T>())
        { }
    }
}
