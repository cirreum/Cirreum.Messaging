namespace Cirreum.Messaging;

/// <summary>
/// A marker interface for a message that can be received from a queue or subscription, encapsulating its metadata and payload.
/// </summary>
/// <remarks>
/// <para>
/// Implementors should implement the <see cref="IMessagingQueuePeekedMessage"/> 
/// and <see cref="IMessagingReceivedMessage"/> interfaces.
/// </para>
/// <para>
/// A message carries three property bags, each with one owner: <see cref="ApplicationProperties"/>
/// belongs to the application, <see cref="SystemProperties"/> to the framework, and
/// <see cref="ProviderProperties"/> to the messaging provider.
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
	/// Gets the application's metadata associated with the message, as the sender supplied it.
	/// </summary>
	IReadOnlyDictionary<string, object> ApplicationProperties { get; }

	/// <summary>
	/// Gets the framework's metadata associated with the message, readable without deserializing the
	/// payload. Keys are reserved — see <see cref="SystemProperty.IsReserved"/>.
	/// </summary>
	/// <remarks>
	/// Values are strings so that a provider's header model cannot alter the shape in transit.
	/// </remarks>
	IReadOnlyDictionary<string, string> SystemProperties { get; }

	/// <summary>
	/// Gets a dictionary of messaging provider-specific properties associated with the message.
	/// </summary>
	IReadOnlyDictionary<string, object> ProviderProperties { get; }

}