using System;
using System.Collections.Generic;

namespace Structura.Shared.MessageBus
{
    internal class MessageHandlerInstances
    {
        public MessageHandlerInstances()
        {
            HandlerTypes = new List<Type>();
        }
        public Type MessageType { get; set; }
        public IList<Type> HandlerTypes { get; set; }
    }
}
