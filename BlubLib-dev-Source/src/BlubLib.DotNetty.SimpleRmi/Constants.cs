using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DotNetty.Common.Utilities;

namespace BlubLib.DotNetty.SimpleRmi
{
    using ResponseQueue = ConcurrentDictionary<Guid, TaskCompletionSource<RmiMessage>>;
    internal static class ChannelAttributes
    {
        public static readonly AttributeKey<ResponseQueue> ResponseQueue =
            AttributeKey<ResponseQueue>.ValueOf($"BlubLib.DotNetty.SimpleRmi-{nameof(ResponseQueue)}");
    }
}
