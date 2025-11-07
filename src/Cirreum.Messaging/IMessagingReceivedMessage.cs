namespace Cirreum.Messaging;

/// <summary>
/// Represents a received message from a queue or subscription, encapsulating its metadata and payload.
/// </summary>
public interface IMessagingReceivedMessage : IMessagingReceivableMessage {

	/// <summary>
	/// Completes a message, removing it from the queue or subscription.
	/// </summary>
	/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous complete operation.</returns>
	Task CompleteMessageAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Abandons a message, returning it to the queue or subscription for redelivery.
	/// </summary>
	/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous abandon operation.</returns>
	Task AbandonMessageAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Defers a message, keeping it in the queue or subscription but hiding it from normal receives.
	/// </summary>
	/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous defer operation.</returns>
	Task DeferMessageAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Moves a message to the dead-letter queue or subscription for later inspection.
	/// </summary>
	/// <param name="reason">The reason for dead-lettering the message.</param>
	/// <param name="description">A detailed description of why the message was dead-lettered.</param>
	/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous dead-letter operation.</returns>
	Task DeadLetterMessageAsync(string reason, string description, CancellationToken cancellationToken = default);

	/// <summary>
	/// Renews the lock on a message to extend processing time.
	/// </summary>
	/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous lock renewal operation.</returns>
	Task RenewLockAsync(CancellationToken cancellationToken = default);

}