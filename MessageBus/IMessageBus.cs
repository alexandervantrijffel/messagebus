namespace Structura.Shared.MessageBus
{
    public interface IMessageBus
    {
        TResult Create<TMsg, TResult>(TMsg args) where TMsg : ICommand;
        void Send<T>(T args) where T:ICommand;
        void Publish<T>(T args) where T:IEvent;
        TResult Request<TMsg, TResult>(TMsg args) where TMsg : IRequest;
    }
}