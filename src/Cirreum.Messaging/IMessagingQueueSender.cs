namespace Cirreum.Messaging;

/// <summary>
/// Represents a sender that publishes messages by sending to a queue in a messaging system.
/// </summary>
public interface IMessagingQueueSender {

	/// <summary>
	/// The name of the queue this sender is associated with.
	/// </summary>
	public string Queue { get; }

	/// <summary>
	/// Publishes an <see cref="OutboundMessage"/> object to a queue.
	/// </summary>
	Task PublishMessageAsync(OutboundMessage message, CancellationToken cancellationToken = default);

	/// <summary>
	/// Publishes a raw string message to a queue (simplified overload).
	/// </summary>
	public Task PublishMessageAsync(string message, CancellationToken cancellationToken = default)
		=> this.PublishMessageAsync(new OutboundMessage(message), cancellationToken);

	/// <summary>
	/// Publishes a raw string message with message properties to a queue (simplified overload).
	/// </summary>
	public Task PublishMessageAsync(string message, IDictionary<string, object> properties, CancellationToken cancellationToken = default)
		=> this.PublishMessageAsync(new OutboundMessage(message, properties), cancellationToken);

	/// <summary>
	/// Publishes multiple <see cref="OutboundMessage"/> objects to a queue.
	/// </summary>
	Task PublishMessagesAsync(IEnumerable<OutboundMessage> messages,
		IDictionary<string, object>? commonProperties = null, CancellationToken cancellationToken = default);

}