using Structura.Shared.MessageBus;

namespace MessageBus.Tests.TestMessages
{
    public class TestCommand : ICommand
    {
        
    }

    // Cannot use a mock but have to write a stub so that the message bus is able to find the type
    public class TestCommandHandler : TestHandlerBase, IHandle<TestCommand>
    {
        public void Handle(TestCommand args)
        {
            ArgsReceived.Add(args);
        }
    }

    public class TestCommandWithoutHandlers : ICommand
    {
        
    }
}