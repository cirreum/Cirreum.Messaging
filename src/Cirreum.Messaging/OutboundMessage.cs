namespace Cirreum.Messaging;

using System.Text;
using System.Text.Json;

/// <summary>
/// Represents a message that can be sent to a message broker's Queue or Topic.
/// </summary>
public class OutboundMessage : IMessagingSendableMessage {

	/// <summary>
	/// The list of properties available in this agnostic <see cref="OutboundMessage"/>.
	/// </summary>
	public static string[] StandardProperties { get; } = [
		"MessageId",    // maps to OutboundMessage.Id
		"Content",
		"ContentString",
		"ContentType",
		"CorrelationId",
		"Subject",
		"TimeToLive",
		"ReplyTo"
	];

	// IMessagingSendableMessage properties

	/// <inheritdoc/>
	public string Id { get; set; } = "";
	/// <summary>
	/// Convenience method to assign the Id and return this instance
	/// for fluent assignment.
	/// </summary>
	/// <param name="id">The unique message identifier.</param>
	/// <returns>The current instance.</returns>
	public OutboundMessage WithId(string id) {
		this.Id = id;
		return this;
	}

	/// <inheritdoc/>
	public byte[] Content { get; }

	/// <summary>
	/// Gets the message content as a UTF-8 string.
	/// </summary>
	public string ContentString {
		get {
			return _contentString ??= Encoding.UTF8.GetString(this.Content);
		}
	}
	string? _contentString;

	/// <inheritdoc/>
	public string ContentType { get; set; } = "";

	/// <inheritdoc/>
	public string CorrelationId { get; set; } = "";
	/// <summary>
	/// Convenience method to assign the CorrelationId and return this instance
	/// for fluent assignment.
	/// </summary>
	/// <param name="correlationId">The correlation identifier used to link related messages in a distributed transaction or workflow.</param>
	/// <returns>The current instance.</returns>
	public OutboundMessage WithCorrelationId(string correlationId) {
		this.CorrelationId = correlationId;
		return this;
	}

	/// <inheritdoc/>
	public string ReplyTo { get; set; } = "";
	/// <summary>
	/// Convenience method to assign the ReplyTo and return this instance
	/// for fluent assignment.
	/// </summary>
	/// <param name="replyTo">The address where replies to this message should be sent.</param>
	/// <returns>The current instance.</returns>
	public OutboundMessage WithReplyTo(string replyTo) {
		this.ReplyTo = replyTo;
		return this;
	}

	/// <inheritdoc/>
	public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();

	/// <inheritdoc/> 
	public IDictionary<string, object> ProviderProperties { get; } = new Dictionary<string, object>();


	// extended properties

	/// <summary>
	/// Gets or sets the duration after which the message expires and should be discarded.
	/// </summary>
	public TimeSpan? TimeToLive { get; set; }
	/// <summary>
	/// The duration after which the message expires and should be discarded.
	/// </summary>
	/// <param name="ttl">The duration.</param>
	/// <returns>The current instance.</returns>
	public OutboundMessage WithTimeToLive(TimeSpan ttl) {
		this.TimeToLive = ttl;
		return this;
	}

	/// <summary>
	/// Gets or sets the subject or topic of the message for filtering and routing.
	/// </summary>
	public string Subject { get; set; } = "";
	/// <summary>
	/// Convenience method to assign the subject and return this instance
	/// for fluent assignment.
	/// </summary>
	/// <param name="subject">The subject or topic of the message for filtering and routing</param>
	/// <returns>The current instance.</returns>
	public OutboundMessage WithSubject(string subject) {
		this.Subject = subject;
		return this;
	}

	// constructors

	/// <summary>
	/// Constructs a message that can be sent to a message broker's Queue or Topic.
	/// </summary>
	/// <param name="content">The raw content of the message.</param>
	public OutboundMessage(byte[] content) {
		this.Content = content;
	}

	/// <summary>
	/// Constructs a message that can be sent to a message broker's Queue or Topic.
	/// </summary>
	/// <param name="content">The raw content of the message from a string.</param>
	public OutboundMessage(string content) {
		this.Content = Encoding.UTF8.GetBytes(content);
	}

	/// <summary>
	/// Constructs a message that can be sent to a message broker's Queue or Topic.
	/// </summary>
	/// <param name="content">The raw content of the message from a string.</param>
	/// <param name="properties">The message specific metadata or attributes associated with the message.</param>
	public OutboundMessage(string content, IDictionary<string, object> properties) {
		this.Content = Encoding.UTF8.GetBytes(content);
		this.Properties = properties;
	}

	/// <summary>
	/// Constructs a message that can be sent to a message broker's Queue or Topic.
	/// </summary>
	/// <param name="content">The raw content of the message from a string.</param>
	/// <param name="properties">The message specific metadata or attributes associated with the message.</param>
	/// <param name="providerProperties">The provider-specific properties for additional message details.</param>
	public OutboundMessage(string content, IDictionary<string, object> properties, IDictionary<string, object> providerProperties) {
		this.Content = Encoding.UTF8.GetBytes(content);
		this.Properties = properties;
		this.ProviderProperties = providerProperties;
	}

	/// <summary>
	/// Creates a new JSON-formatted message from the specified object.
	/// </summary>
	/// <typeparam name="T">The type of object to serialize.</typeparam>
	/// <param name="message">The object to serialize as the message content.</param>
	/// <returns>A new <see cref="OutboundMessage"/> with JSON-serialized content and "application/json" content type.</returns>
	/// <exception cref="JsonException">Thrown when serialization of the message fails.</exception>
	public static OutboundMessage AsJsonContent<T>(T message)
		where T : notnull {
		return new(JsonSerializer.Serialize(message)) {
			ContentType = "application/json"
		};
	}

	/// <summary>
	/// Creates a new JSON-formatted message from the specified object.
	/// </summary>
	/// <typeparam name="T">The type of object to serialize.</typeparam>
	/// <param name="message">The object to serialize as the message content.</param>
	/// <param name="properties">The message specific metadata or attributes associated with the message.</param>
	/// <returns>A new <see cref="OutboundMessage"/> with JSON-serialized content and "application/json" content type.</returns>
	/// <exception cref="JsonException">Thrown when serialization of the message fails.</exception>
	public static OutboundMessage AsJsonContent<T>(
		T message,
		IDictionary<string, object> properties)
		where T : notnull {
		return new(JsonSerializer.Serialize(message), properties) {
			ContentType = "application/json"
		};
	}
	/// <summary>
	/// Creates a new JSON-formatted message from the specified object.
	/// </summary>
	/// <typeparam name="T">The type of object to serialize.</typeparam>
	/// <param name="message">The object to serialize as the message content.</param>
	/// <param name="properties">The message specific metadata or attributes associated with the message.</param>
	/// <param name="providerProperties">The provider-specific properties for additional message details.</param>
	/// <returns>A new <see cref="OutboundMessage"/> with JSON-serialized content and "application/json" content type.</returns>
	/// <exception cref="JsonException">Thrown when serialization of the message fails.</exception>
	public static OutboundMessage AsJsonContent<T>(
		T message,
		IDictionary<string, object> properties,
		IDictionary<string, object> providerProperties)
		where T : notnull {
		return new(JsonSerializer.Serialize(message), properties, providerProperties) {
			ContentType = "application/json"
		};
	}

}