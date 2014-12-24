using System;

namespace Structura.Shared.MessageBus
{
    public static class MessageBusAccessor
    {
        private static readonly IMessageBus instance = new SynchronousInMemoryMessageBus();
        public static Func<IMessageBus> Instance;
        
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static MessageBusAccessor()
        {
            RevertToOriginalInstance();
        }

        /// <summary>
        /// For unit testing blegh
        /// </summary>
        public static void RevertToOriginalInstance()
        {
            Instance = () => instance;
        }
    }
}
