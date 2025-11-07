namespace Cirreum.Messaging;

/// <summary>
/// A marker interface for a message that can be sent to a queue or
/// topic, encapsulating its metadata and payload.
/// </summary>
public interface IMessagingSendableMessage : IBaseMessage {

	/// <summary>
	/// Gets a dictionary of message specific metadata or attributes to include with the message,
	/// if the provider support application level properties.
	/// </summary>
	IDictionary<string, object> Properties { get; }

	/// <summary>
	/// Gets a dictionary of messaging provider-specific properties that can be consumed by the provider
	/// to further set other message specifc properies.
	/// </summary>
	IDictionary<string, object> ProviderProperties { get; }

}