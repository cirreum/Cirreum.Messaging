namespace Cirreum.Messaging;

/// <summary>
/// A base interface with common properties for any message.
/// </summary>
public interface IBaseMessage {

	/// <summary>
	/// Gets the unique message identifier.
	/// </summary>
	string Id { get; }

	/// <summary>
	/// Gets the message content as a byte[].
	/// </summary>
	byte[] Content { get; }

	/// <summary>
	/// Gets the message content as a UTF-8 string.
	/// </summary>
	string ContentString { get; }

	/// <summary>
	/// Gets the message content type (e.g., "application/json", "text/plain").
	/// </summary>
	string ContentType { get; }

	/// <summary>
	/// Gets the correlation identifier used to link related messages in a distributed transaction or workflow.
	/// </summary>
	string CorrelationId { get; }

	/// <summary>
	/// Gets the address where replies to this message should be sent.
	/// </summary>
	string ReplyTo { get; }

}