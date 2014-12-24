using System.Collections.Generic;
using System.Reflection;

namespace Structura.Shared.MessageBus
{
    public interface IMessageBus
    {
        void RegisterHandlers(IMessageHandlerResolver container, IList<Assembly> commandHandlersAssemblies, IEnumerable<Assembly> requestHandlersAssemblies);
        TResult Create<TMsg, TResult>(TMsg args) where TMsg : ICommand;
        void Send<T>(T args) where T:ICommand;
        void Publish<T>(T args) where T:IEvent;
        TResult Request<TMsg, TResult>(TMsg args) where TMsg : IRequest;
    }
}