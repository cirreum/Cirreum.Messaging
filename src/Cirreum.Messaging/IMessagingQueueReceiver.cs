namespace Cirreum.Messaging;

/// <summary>
/// 
/// </summary>
public interface IMessagingQueueReceiver {

	/// <summary>
	/// The name of the queue this receiver is associated with.
	/// </summary>
	public string Queue { get; }

	// Peek

	/// <summary>
	/// Peeks an <see cref="IMessagingQueueReceivedMessage"/> message from the queue without locking or
	/// consuming it.
	/// </summary>
	Task<IMessagingQueuePeekedMessage> PeekMessageAsync(
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Peeks multiple <see cref="IMessagingQueueReceivedMessage"/> messages from the queue without locking
	/// or consuming them.
	/// </summary>
	Task<IReadOnlyList<IMessagingQueuePeekedMessage>> PeekMessagesAsync(
		int maxMessages,
		CancellationToken cancellationToken = default);


	// Receive

	/// <summary>
	/// Receives an <see cref="IMessagingQueueReceivedMessage"/> message from the queue.
	/// </summary>
	/// <param name="maxWaitTime">An optional <see cref="TimeSpan"/> specifying the maximum time to wait
	/// for a message before returning null if no messages are available.</param>
	/// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal
	/// the request to cancel the operation.</param>
	/// <returns>The message received. Returns null if no message is found.</returns>
	Task<IMessagingQueueReceivedMessage> ReceiveMessageAsync(
		TimeSpan? maxWaitTime = default,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Receives multiple <see cref="IMessagingQueueReceivedMessage"/> messages as an asynchronous enumerable
	/// from the queue. Messages will be received from the queue as the IAsyncEnumerable is iterated. If no
	/// messages are available, this method will continue polling until messages are available, i.e. it will
	/// never return null.
	/// </summary>
	/// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal the
	/// request to cancel the operation.</param>
	/// <returns>The message received.</returns>
	IAsyncEnumerable<IMessagingQueueReceivedMessage> ReceiveMessagesStreamAsync(
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Receives a list of <see cref="IMessagingQueueReceivedMessage"/> messages from the queue. This method
	/// doesn't guarantee to return exact `maxMessages` messages, even if there are `maxMessages` messages
	/// available in the queue or topic.
	/// </summary>
	/// <param name="maxMessages">The maximum number of messages that will be received.</param>
	/// <param name="maxWaitTime">An optional <see cref="TimeSpan"/> specifying the maximum time to
	/// wait for the first message before returning an empty list if no messages are available.</param>
	/// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal the
	/// request to cancel the operation.</param>
	/// <returns>List of messages received. Returns an empty list if no message is found.</returns>
	Task<IReadOnlyList<IMessagingQueueReceivedMessage>> ReceiveMessagesAsync(
		int maxMessages,
		TimeSpan? maxWaitTime = default,
		CancellationToken cancellationToken = default);


}