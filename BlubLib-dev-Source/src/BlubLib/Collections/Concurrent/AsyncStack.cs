using System.Collections.Concurrent;

namespace BlubLib.Collections.Concurrent
{
    public class AsyncStack<T> : AsyncCollection<T>
    {
        public AsyncStack()
            : base(new ConcurrentStack<T>())
        { }
    }
}
