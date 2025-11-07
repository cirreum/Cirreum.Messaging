namespace Cirreum.Messaging;

/// <summary>
/// A marker interface for a message that can be received from a queue or subscription, encapsulating its metadata and payload.
/// </summary>
/// <remarks>
/// <para>
/// Implementors should implement the <see cref="IMessagingQueuePeekedMessage"/> 
/// and <see cref="IMessagingReceivedMessage"/> interfaces.
/// </para>
/// </remarks>
public interface IMessagingReceivableMessage
	: IBaseMessage {

	/// <summary>
	/// Gets the date and time when the message was enqueued in the queue or subscription.
	/// </summary>
	DateTimeOffset EnqueuedTime { get; }

	/// <summary>
	/// Gets the date and time when this message will expire and be automatically removed from the queue or subscription.
	/// </summary>
	DateTimeOffset ExpiresAt { get; }

	/// <summary>
	/// Gets the number of times this message has been delivered to receivers.
	/// </summary>
	int DeliveryCount { get; }

	/// <summary>
	/// Gets a dictionary of message specific metadata or attributes associated with the message.
	/// </summary>
	IReadOnlyDictionary<string, object> Properties { get; }

	/// <summary>
	/// Gets a dictionary of messaging provider-specific properties associated with the message.
	/// </summary>
	IReadOnlyDictionary<string, object> ProviderProperties { get; }

}