using System.Collections.Generic;

namespace MessageBus.Tests
{
    public class TestHandlerBase
    {
        public static IList<dynamic> ArgsReceived { get; set; }
        public const int DefaultReturnValue = -1;

        public TestHandlerBase()
        {
            ArgsReceived = new List<dynamic>();
        }
    }
}