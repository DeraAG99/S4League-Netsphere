using System.Threading;
using DotNetty.Transport.Channels;

namespace BlubLib.DotNetty.SimpleRmi
{
    public abstract class RmiService
    {
        private readonly AsyncLocal<IChannelHandlerContext> _currentContext = new AsyncLocal<IChannelHandlerContext>();

        public IChannelHandlerContext CurrentContext
        {
            get { return _currentContext.Value; }
            internal set { _currentContext.Value = value; }
        }

        //public RmiService()
        //{
        //    _currentContext = new AsyncLocal<IChannelHandlerContext>(e =>
        //    {
        //        Console.WriteLine("Current:{0} Prev:{1} Context:{2}", e.CurrentValue != null ? "" : "NULL", e.PreviousValue != null ? "" : "NULL", e.ThreadContextChanged);
        //    });
        //}
    }
}