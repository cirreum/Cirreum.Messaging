namespace Cirreum.Messaging;

/// <summary>
/// A marker interface for a message that can be sent to a queue or
/// topic, encapsulating its metadata and payload.
/// </summary>
/// <remarks>
/// A message carries three property bags, each with one owner: <see cref="ApplicationProperties"/>
/// belongs to the application, <see cref="SystemProperties"/> to the framework, and
/// <see cref="ProviderProperties"/> to the messaging provider.
/// </remarks>
public interface IMessagingSendableMessage : IBaseMessage {

	/// <summary>
	/// Gets the application's metadata to include with the message, if the provider supports
	/// application-level properties.
	/// </summary>
	/// <remarks>
	/// A provider forwards these entries as supplied. It does not add, rename, or remove one.
	/// </remarks>
	IDictionary<string, object> ApplicationProperties { get; }

	/// <summary>
	/// Gets the framework's metadata to travel with the message, readable by a receiver without
	/// deserializing the payload. Keys are reserved — see <see cref="SystemProperty.IsReserved"/>.
	/// </summary>
	/// <remarks>
	/// Values are strings so that a provider's header model cannot alter the shape in transit. A
	/// provider forwards these entries as supplied, and never filters one.
	/// </remarks>
	IDictionary<string, string> SystemProperties { get; }

	/// <summary>
	/// Gets a dictionary of messaging provider-specific properties that can be consumed by the provider
	/// to further set other message specific properties.
	/// </summary>
	IDictionary<string, object> ProviderProperties { get; }

}
