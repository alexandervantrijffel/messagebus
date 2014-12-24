using System;
using System.Linq;
using FluentAssertions;
using MessageBus.Tests.TestMessages;
using NSubstitute;
using Structura.Shared.MessageBus;
using Xunit;

namespace MessageBus.Tests
{
    public class WhenSending : IUseFixture<MessageBusInitializerFixture>
    {
        private MessageBusInitializerFixture _messageBusInitializer;
        public void SetFixture(MessageBusInitializerFixture messageBusInitializer)
        {
            _messageBusInitializer = messageBusInitializer;
            _messageBusInitializer.ClearReceivedCalls();
        }

        [Fact]
        public void TestCommand_should_invoke_TestCommandHandler_handle()
        {
            // Arrange
            var command = new TestCommand();
            // Act
            MessageBusAccessor.Instance().Send(command);

            // Assert
            VerifyMessageProcessing(typeof(TestCommandHandler), command.GetType());
        }

        [Fact]
        public void TestCommandWithoutHandlers_should_raise_exception()
        {
            // Arrange
            Action a = () => MessageBusAccessor.Instance().Send(new TestCommandWithoutHandlers());

            // Act
            // Assert
            a.ShouldThrow<PreconditionException>()
                .Which.Message.StartsWith("No handlers found for message type TestCommandWithoutHandlers. At method ")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void TestRequest_should_invoke_TestRequestHandler_handle()
        {
            // Arrange
            var request = new TestRequest();
            // Act
            var result = MessageBusAccessor.Instance().Request<TestRequest,int>(request);

            // Assert
            VerifyMessageProcessing(typeof(TestRequestHandler), request.GetType());
            result.Should().Be(TestRequestHandler.DefaultReturnValue);

        }
        [Fact]
        public void TestCreate_should_invoke_TestCreateHandler_handle()
        {
            // Arrange
            var create = new TestCreate();
            // Act
            var result = MessageBusAccessor.Instance().Create<TestCreate,int>(create);

            // Assert
            VerifyMessageProcessing(typeof(TestCreateHandler), create.GetType());
            result.Should().Be(TestCreateHandler.DefaultReturnValue);
        }

        [Fact]
        public void TestPublish_should_invoke_TestPublishHandler_handle()
        {
            // Arrange
            var create = new TestPublish();
            // Act
            MessageBusAccessor.Instance().Publish(create);

            // Assert
            VerifyMessageProcessing(typeof(TestPublishHandler), create.GetType());
        }

        private void VerifyMessageProcessing(Type handlerType, Type messageType)
        {
            _messageBusInitializer.MessageHandlerResolverMock.Received(1).Resolve(handlerType);
            TestHandlerBase.ArgsReceived.Count.Should().Be(1);
            var arg = TestHandlerBase.ArgsReceived.First();
            AssertionExtensions.ShouldBeEquivalentTo(arg.GetType(), messageType);
        }
    }
}
