using Structura.Shared.MessageBus;

namespace MessageBus.Tests.TestMessages
{
    public class TestRequest : IRequest
    {
    }

    public class TestRequestHandler : TestHandlerBase,IHandleRequest<TestRequest, int>
    {
        public int Get(TestRequest args)
        {
            ArgsReceived.Add(args);
            return DefaultReturnValue;
        }
    }
}
