namespace Cirreum.Messaging;

/// <summary>
/// Represents a sender that broadcasts messages by sending to a topic in a messaging system.
/// </summary>
public interface IMessagingTopicSender {

	/// <summary>
	/// Gets the name of the topic to which messages will be broadcast.
	/// </summary>
	string Topic { get; }


	/// <summary>
	/// Broadcast an <see cref="OutboundMessage"/> object to a topic.
	/// </summary>
	Task BroadcastMessageAsync(OutboundMessage message, CancellationToken cancellationToken = default);

	/// <summary>
	/// Broadcast a raw string message to a topic (simplified overload).
	/// </summary>
	public Task BroadcastMessageAsync(string message, CancellationToken cancellationToken = default)
		=> this.BroadcastMessageAsync(new OutboundMessage(message), cancellationToken);

	/// <summary>
	/// Broadcast a raw string message to a topic with additional properties (simplified overload).
	/// </summary>
	public Task BroadcastMessageAsync(string message, IDictionary<string, object> properties, CancellationToken cancellationToken = default)
		=> this.BroadcastMessageAsync(new OutboundMessage(message, properties), cancellationToken);

	/// <summary>
	/// Broadcast multiple <see cref="OutboundMessage"/> objects to a topic.
	/// </summary>
	Task BroadcastMessagesAsync(IEnumerable<OutboundMessage> messages,
		IDictionary<string, object>? commonProperties = null, CancellationToken cancellationToken = default);

}