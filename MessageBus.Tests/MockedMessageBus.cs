using System;
using NSubstitute;
using Structura.Shared.MessageBus;

namespace DomainProcesses.Tests
{
    /// <summary>
    /// Make sure that instances of MockedBus are disposed at the end of the test so that the MessageBusAccessor.Instance() is reset
    /// </summary>
    public class MockedMessageBus : IDisposable
    {
        private IMessageBus _originalMessageBus;
        public IMessageBus MockedBus { get; set; }
        public MockedMessageBus()
        {
            _originalMessageBus = MessageBusAccessor.Instance;
            MockedBus = Substitute.For<IMessageBus>();
            MessageBusAccessor.SetMessageBusResolver(() => MockedBus);
        }

        public void Dispose()
        {
            MessageBusAccessor.SetMessageBusResolver(() => _originalMessageBus);
        }
    }
}
