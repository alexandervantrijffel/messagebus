using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Structura.Shared.MessageBus
{
    public static class MessageBusAccessor
    {
        private static Func<IMessageBus> _instanceResolver;


        public static IMessageBus Instance { get { return _instanceResolver(); }}
        
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1409:RemoveUnnecessaryCode", 
            Justification = "Explicit static constructor to tell C# compiler not to mark type as beforefieldinit")]
        static MessageBusAccessor()
        {
        }
        /// <summary>
        /// Override the default message bus instance
        /// </summary>
        public static void SetMessageBusResolver(Func<IMessageBus> messageBusResolver)
        {
            _instanceResolver = messageBusResolver;
        }

        public static void Initialize(IMessageHandlerResolver resolver, IEnumerable<Assembly> commandAndEventHandlersAssemblies, IEnumerable<Assembly> requestHandlersAssemblies)
        {
            _instanceResolver =  () => new SynchronousInMemoryMessageBus(resolver,commandAndEventHandlersAssemblies,requestHandlersAssemblies);
        }
    }
}
