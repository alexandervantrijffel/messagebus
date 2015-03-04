using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Structura.Shared.MessageBus
{
    internal class SynchronousInMemoryMessageBus : IMessageBus
    {
        private static IMessageHandlerResolver _resolver;
        private static IList<MessageHandlerInstances> _cachedHandlerTypes;
        private static IList<Type> _handlerOrder;

        public SynchronousInMemoryMessageBus(IMessageHandlerResolver resolver, IEnumerable<Assembly> commandAndEventHandlersAssemblies, IEnumerable<Assembly> requestHandlersAssemblies)
        {
            _resolver = resolver;
            _cachedHandlerTypes = new List<MessageHandlerInstances>();
            _cachedHandlerTypes.Clear();

            var handlersAssemblies = commandAndEventHandlersAssemblies as Assembly[] ?? commandAndEventHandlersAssemblies.ToArray();
            RegisterHandlersWithReturnType(handlersAssemblies, typeof(IHandle<>));
            RegisterHandlersWithReturnType(handlersAssemblies, typeof(ICreate<,>));
            RegisterHandlersWithReturnType(requestHandlersAssemblies, typeof(IHandleRequest<,>));
        }

        private static void RegisterHandlersWithReturnType(IEnumerable<Assembly> commandHandlersAssemblies, Type type)
        {
            foreach (var a in commandHandlersAssemblies)
            {
                foreach (var t in a.GetTypes())
                {
                    foreach (var i in t.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == type))
                    { RegisterHandler(i.GenericTypeArguments[0], t); }
                }
            }
        }

        private static void RegisterHandler(Type msgType, Type handlerType)
        {
            var cacheItem = _cachedHandlerTypes.SingleOrDefault(c => c.MessageType == msgType);
            if (cacheItem == null)
            {
                cacheItem = new MessageHandlerInstances { MessageType = msgType };
                _cachedHandlerTypes.Add(cacheItem);
            }
            cacheItem.HandlerTypes.Add(handlerType);

            // sort the handlers
            if (_handlerOrder != null)
            {
                var priorityHandlers = new List<Type>();
                foreach (Type handlerTmp in _handlerOrder)
                {
                    for (var j = 0; j < cacheItem.HandlerTypes.Count; j++)
                    {
                        if (cacheItem.HandlerTypes[j] == handlerTmp)
                        {
                            // prio's are stored in reverse order
                            priorityHandlers.Insert(0, cacheItem.HandlerTypes[j]);
                            cacheItem.HandlerTypes.RemoveAt(j);
                        }
                    }
                }
                foreach (var prio in priorityHandlers)
                {
                    cacheItem.HandlerTypes.Insert(0, prio);
                }
            }
        }
        public void Publish<TMsg>(TMsg args) where TMsg : IEvent
        {
            var handlers = GetHandlers<TMsg>();
            if (handlers != null && handlers.Any())
                foreach (var handler in handlers)
                    Invoke("Handle", handler, args);
        }

        public void Send<TMsg>(TMsg args) where TMsg : ICommand
        {
            var handlers = GetHandlers<TMsg>();
            MessageBusCheck.Require(handlers != null && handlers.Any(), "No handlers found for message type " + args.GetType().Name);
            foreach (var handler in handlers)
                Invoke("Handle", handler, args);
        }

        public TResult Create<TMsg, TResult>(TMsg args) where TMsg : ICommand
        {
            var handlers = GetHandlers<TMsg>();
            MessageBusCheck.Require(handlers != null && handlers.Any(), "No handlers found for message type " + args.GetType().Name);
            object result = null;
            foreach (var handler in handlers)
            {
                object newResult = Invoke("Handle", handler, args);
                MessageBusCheck.Require(newResult == null || result == null, "Only one handler for a Create message can return a value, found multiple return objects for message {0}.", args.GetType().Name);
                if (newResult != null) result = newResult;
            }
            return (TResult)result;
        }

        public TResult Request<TMsg, TResult>(TMsg args) where TMsg : IRequest
        {
            var handlers = GetHandlers<TMsg>();
            MessageBusCheck.Require(handlers != null && handlers.Any(), "No handlers found for message type " + args.GetType().Name);
            MessageBusCheck.Require(handlers.Count() == 1, "Expected exactly one handler for request " + args.GetType().Name + ". Found " + handlers.Count() + " handlers.");
            return (TResult)Invoke("Get", handlers.First(), args);
        }

        private static object Invoke(string methodName, object instance, object args)
        {
            var method = instance.GetType().GetMethod(methodName);
            try
            {
                return method.Invoke(instance, new[] { args });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }
        private static IList<object> GetHandlers<TMsg>() where TMsg : IMsg
        {
            var list = new List<object>();
            foreach (var handlerType in _cachedHandlerTypes)
            {
                if (handlerType.MessageType.IsAssignableFrom(typeof(TMsg)))
                {
                    foreach (var item in handlerType.HandlerTypes)
                    {
                        list.Add(_resolver.Resolve(item));
                    }

                }
            }
            return list;
        }
    }
}
