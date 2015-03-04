using System;
using MessageBus.Tests.TestMessages;
using NSubstitute;
using Structura.Shared.MessageBus;

namespace MessageBus.Tests
{
    public class MessageBusInitializerFixture : IDisposable
    {
        public IMessageHandlerResolver MessageHandlerResolverMock { get; private set; }

        public MessageBusInitializerFixture()
        {
            MessageHandlerResolverMock = Substitute.For<IMessageHandlerResolver>();
            var messageHandlingAssemblyList = new[] {typeof (TestCommand).Assembly};

            MessageBusAccessor.Initialize(MessageHandlerResolverMock,messageHandlingAssemblyList,messageHandlingAssemblyList);
            MessageHandlerResolverMock.Resolve(Arg.Any<Type>()).Returns(c => Activator.CreateInstance((Type) c.Args()[0]));
        }

        public void ClearReceivedCalls()
        {
            MessageHandlerResolverMock.ClearReceivedCalls();
        }
        public void Dispose()
        {
        }
    }
}