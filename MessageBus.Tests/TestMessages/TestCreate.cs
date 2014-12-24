using Structura.Shared.MessageBus;

namespace MessageBus.Tests.TestMessages
{
    public class TestCreate : ICommand
    {
    }

    public class TestCreateHandler : TestHandlerBase, ICreate<TestCreate, int>
    {
        public int Handle(TestCreate msg)
        {
            ArgsReceived.Add(msg);
            return DefaultReturnValue;
        }
    }
}
