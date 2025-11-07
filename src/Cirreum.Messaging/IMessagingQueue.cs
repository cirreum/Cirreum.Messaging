namespace Cirreum.Messaging;

/// <summary>
/// Represents a message queue that provides both sending and receiving capabilities.
/// Combines the functionality of <see cref="IMessagingQueueSender"/> and 
/// <see cref="IMessagingQueueReceiver"/> into a single interface, while providing
/// access to the underlying sender and receiver implementations.
/// </summary>
public interface IMessagingQueue
   : IMessagingQueueSender
   , IMessagingQueueReceiver;