namespace Cirreum.Messaging;

/// <summary>
/// Represents a peeked message from a queue, encapsulating its metadata and payload.
/// </summary>
public interface IMessagingQueuePeekedMessage : IMessagingReceivableMessage {
	/// <summary>
	/// Gets the name of the queue from which this message was received.
	/// </summary>
	string Queue { get; }
}