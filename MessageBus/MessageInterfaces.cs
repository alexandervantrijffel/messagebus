namespace Structura.Shared.MessageBus
{
    public partial interface IMsg
    {

    }

    public partial interface ICommand : IMsg
    {
         
    }

    public partial interface IRequest : IMsg
    {
    }

    public partial interface IEvent : IMsg
    {

    }
}