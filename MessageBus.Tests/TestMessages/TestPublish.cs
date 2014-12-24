using Structura.Shared.MessageBus;

namespace MessageBus.Tests.TestMessages
{
    public class TestPublish : IEvent
    {
    }

    public class TestPublishHandler : TestHandlerBase, IHandle<TestPublish>
    {
        public void Handle(TestPublish args)
        {
            ArgsReceived.Add(args);
        }
    }
}
