messagebus
==========

The message bus is a component for streamlined communication within a system. It allows for decoupling components and objects by communicating through an message channel. With the message bus clients and server are unaware of each other and can be developed in isolation. The messages define the contract between server and client.

The bus supports the following messages methods:

Message | Description | Message type | Handler type | Return value
--- | --------------- | ----- | ----- | -----
Send &lt;TMsg>  | Send a command message | ICommand | IHandle &lt;TMsg> | void
Create &lt;TMsg,TResult> |  Send a command message and retrieve the created object | ICommand | ICreate &lt;TMsg,TResult> | TResult
Publish &lt;TMsg> | Send an event message to all subscribers of the event (publish-subscribe) | IEvent | IHandle &lt;TMsg> | void
Request &lt;TObject> | Request an object | IRequest | IHandleRequest &lt;TMsg,TObject>| TObject

Messages can be sent using the MessageBusAccessor. Example: 

```
MessageBusAccessor.Instance().Send(new MyCommand())
```

The bus dispatches the message to all message handlers for the given message type and message method. The message is dispatched to each handler sequentially. This is not a durable message bus. All message handling is performed in-memory and synchronously. Each handler can interrupt the message processing by throwing an exception.

While providing convenient access to the bus, the MessageBusAccessor can still be replaced with a mock for unit tests because th Instance method is a delegate.

Maintained by Alexander van Trijffel of [Software Development Consultancy company Structura](http://structura.ws)
