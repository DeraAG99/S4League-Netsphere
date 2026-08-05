using System.Collections.Concurrent;

namespace BlubLib.Collections.Concurrent
{
    public class AsyncQueue<T> : AsyncCollection<T>
    {
        public AsyncQueue()
            : base(new ConcurrentQueue<T>())
        { }
    }
}
