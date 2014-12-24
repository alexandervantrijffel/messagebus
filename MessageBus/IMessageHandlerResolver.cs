using System;

namespace Structura.Shared.MessageBus
{
    public interface IMessageHandlerResolver
    {
        object Resolve(Type t);
    }
}
