namespace Cirreum.Messaging;

/// <summary>
/// Marker interface for use with Subscription messaging.
/// </summary>
public interface IMessagingSubscriptionReceiver {

	/// <summary>
	/// Gets the name of the topic from which messages are received.
	/// </summary>
	string Topic { get; }

	/// <summary>
	/// Gets the name of the subscription to which this receiver is bound.
	/// </summary>
	string Subscription { get; }


	// Peek

	/// <summary>
	/// Peeks a message from the queue without locking or consuming it.
	/// </summary>
	/// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal request 
	/// cancellation.</param>
	Task<IMessagingSubscriptionPeekedMessage> PeekMessageAsync(
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Peeks multiple messages from the queue without locking or consuming them.
	/// </summary>
	/// <param name="maxMessages">The maximum number of messages to receive.</param>
	/// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal request 
	/// cancellation.</param>
	Task<IReadOnlyList<IMessagingSubscriptionPeekedMessage>> PeekMessagesAsync(
		int maxMessages,
		CancellationToken cancellationToken = default);

	// Receive

	/// <summary>
	/// Receives a single message from the subscription.
	/// </summary>
	/// <param name="maxWaitTime">An optional <see cref="TimeSpan"/> specifying the maximum time to wait for
	/// a message before returning null.</param>
	/// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal request
	/// cancellation.</param>
	/// <returns>The received message as <see cref="IMessagingSubscriptionReceivedMessage"/> or null if no message
	/// is available.</returns>
	Task<IMessagingSubscriptionReceivedMessage> ReceiveMessageAsync(
		TimeSpan? maxWaitTime = default,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Receives messages as an asynchronous enumerable from the subscription.
	/// </summary>
	/// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal request
	/// cancellation.</param>
	/// <returns>An asynchronous enumerable of received messages.</returns>
	IAsyncEnumerable<IMessagingSubscriptionReceivedMessage> ReceiveMessagesStreamAsync(
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Receives multiple messages from the subscription in a single batch operation.
	/// </summary>
	/// <param name="maxMessages">The maximum number of messages to receive.</param>
	/// <param name="maxWaitTime">An optional <see cref="TimeSpan"/> specifying the maximum time to wait for messages
	/// before returning an empty list.</param>
	/// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal request 
	/// cancellation.</param>
	/// <returns>A list of received messages. Returns an empty list if no messages are available.</returns>
	Task<IReadOnlyList<IMessagingSubscriptionReceivedMessage>> ReceiveMessagesAsync(
		int maxMessages,
		TimeSpan? maxWaitTime = default,
		CancellationToken cancellationToken = default);

}