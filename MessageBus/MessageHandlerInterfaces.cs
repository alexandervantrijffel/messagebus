namespace Structura.Shared.MessageBus
{
    public interface IHandle<T> where T : IMsg
    {
        void Handle(T args);
    }

    public interface ICreate<TMsg, TResult> where TMsg : ICommand
    {
        TResult Handle(TMsg args);
    }

    public interface IHandleRequest<TRequest, TResult>
    where TRequest : IRequest
    {
        TResult Get(TRequest args);
    }
}

