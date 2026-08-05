using System;
using BlubLib.Serialization;

namespace BlubLib.DotNetty.SimpleRmi
{
    [BlubContract]
    public class RmiMessage
    {
        [BlubMember(0)]
        public Guid Guid { get; set; }
    }

    [BlubContract]
    public class KeepAliveMessage : RmiMessage
    { }
}