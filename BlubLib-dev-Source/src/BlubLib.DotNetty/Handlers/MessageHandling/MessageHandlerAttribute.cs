using System;

namespace BlubLib.DotNetty.Handlers.MessageHandling
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MessageHandlerAttribute : Attribute
    {
        internal object MessageId { get; }

        public MessageHandlerAttribute(object messageId)
        {
            MessageId = messageId ?? throw new ArgumentNullException(nameof(messageId));
        }
    }
}