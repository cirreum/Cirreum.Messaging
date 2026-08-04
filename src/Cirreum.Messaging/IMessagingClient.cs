namespace Cirreum.Messaging;

/// <summary>
/// Provides factory methods for creating messaging clients to interact with queues, topics and subscriptions.
/// </summary>
public interface IMessagingClient {

	/// <summary>
	/// Creates or retrieves a cached message queue that provides both sending and receiving capabilities.
	/// </summary>
	/// <param name="queue">The name of the queue to use.</param>
	/// <returns>An <see cref="IMessagingQueue"/> implementation that enables sending messages to and receiving messages from the specified queue.</returns>
	IMessagingQueue UseQueue(string queue);

	/// <summary>
	/// Creates or retrieves a cached queue receiver for consuming messages.
	/// </summary>
	/// <param name="queue">The name of the queue to receive messages from.</param>
	/// <returns>An <see cref="IMessagingQueueReceiver"/> implementation that enables receiving messages from the specified queue.</returns>
	IMessagingQueueReceiver UseQueueReceiver(string queue);

	/// <summary>
	/// Creates a queue receiver with broker-level tuning applied at creation, or retrieves
	/// the cached receiver when one was already created for <paramref name="queue"/> with
	/// equal tuning.
	/// </summary>
	/// <param name="queue">The name of the queue to receive messages from.</param>
	/// <param name="tuning">
	/// Broker-level receiver tuning. Values the instance does not set fall back to the
	/// broker's own defaults.
	/// </param>
	/// <returns>An <see cref="IMessagingQueueReceiver"/> implementation that enables receiving messages from the specified queue.</returns>
	/// <remarks>
	/// <para>
	/// Receivers are cached by name, so tuning participates in the first creation only.
	/// Implementations must throw when the same queue is requested again with different
	/// tuning rather than silently returning the previously created instance.
	/// </para>
	/// <para>
	/// The default implementation delegates to <see cref="UseQueueReceiver(string)"/> when
	/// <paramref name="tuning"/> carries no values, and throws
	/// <see cref="NotSupportedException"/> when it does — an implementation that has not
	/// opted into tuning fails loudly rather than silently discarding it.
	/// </para>
	/// </remarks>
	/// <exception cref="NotSupportedException">
	/// The implementation does not support receiver tuning and <paramref name="tuning"/>
	/// carries at least one explicit value.
	/// </exception>
	IMessagingQueueReceiver UseQueueReceiver(string queue, ReceiverTuning tuning) {
		ArgumentNullException.ThrowIfNull(tuning);
		if (tuning.IsEmpty) {
			return this.UseQueueReceiver(queue);
		}
		throw new NotSupportedException(
			$"{this.GetType().Name} does not support receiver tuning. " +
			"Override UseQueueReceiver(string, ReceiverTuning) to map the values onto the broker.");
	}

	/// <summary>
	/// Creates or retrieves a cached queue sender for publishing messages.
	/// </summary>
	/// <param name="queue">The name of the queue to send messages to.</param>
	/// <returns>An <see cref="IMessagingQueueSender"/> implementation that enables sending messages to the specified queue.</returns>
	IMessagingQueueSender UseQueueSender(string queue);

	/// <summary>
	/// Creates or retrieves a cached topic sender for publishing messages.
	/// </summary>
	/// <param name="topic">The name of the topic to publish messages to.</param>
	/// <returns>An <see cref="IMessagingTopicSender"/> implementation that enables publishing messages to the specified topic.</returns>
	IMessagingTopicSender UseTopic(string topic);

	/// <summary>
	/// Creates or retrieves a cached subscription receiver for consuming messages from a topic.
	/// </summary>
	/// <param name="topic">The name of the topic.</param>
	/// <param name="subscription">The name of the subscription within the topic.</param>
	/// <returns>An <see cref="IMessagingSubscriptionReceiver"/> implementation that enables receiving messages from the specified topic subscription.</returns>
	IMessagingSubscriptionReceiver UseSubscription(string topic, string subscription);

	/// <summary>
	/// Creates a subscription receiver with broker-level tuning applied at creation, or
	/// retrieves the cached receiver when one was already created for the same
	/// <paramref name="topic"/> and <paramref name="subscription"/> with equal tuning.
	/// </summary>
	/// <param name="topic">The name of the topic.</param>
	/// <param name="subscription">The name of the subscription within the topic.</param>
	/// <param name="tuning">
	/// Broker-level receiver tuning. Values the instance does not set fall back to the
	/// broker's own defaults.
	/// </param>
	/// <returns>An <see cref="IMessagingSubscriptionReceiver"/> implementation that enables receiving messages from the specified topic subscription.</returns>
	/// <remarks>
	/// <para>
	/// Receivers are cached by name, so tuning participates in the first creation only.
	/// Implementations must throw when the same subscription is requested again with
	/// different tuning rather than silently returning the previously created instance.
	/// </para>
	/// <para>
	/// The default implementation delegates to <see cref="UseSubscription(string, string)"/>
	/// when <paramref name="tuning"/> carries no values, and throws
	/// <see cref="NotSupportedException"/> when it does — an implementation that has not
	/// opted into tuning fails loudly rather than silently discarding it.
	/// </para>
	/// </remarks>
	/// <exception cref="NotSupportedException">
	/// The implementation does not support receiver tuning and <paramref name="tuning"/>
	/// carries at least one explicit value.
	/// </exception>
	IMessagingSubscriptionReceiver UseSubscription(string topic, string subscription, ReceiverTuning tuning) {
		ArgumentNullException.ThrowIfNull(tuning);
		if (tuning.IsEmpty) {
			return this.UseSubscription(topic, subscription);
		}
		throw new NotSupportedException(
			$"{this.GetType().Name} does not support receiver tuning. " +
			"Override UseSubscription(string, string, ReceiverTuning) to map the values onto the broker.");
	}

	/// <summary>
	/// Provides access to the provider-specific client for custom operations not supported by the standard interfaces.
	/// </summary>
	/// <typeparam name="T">The type of the provider-specific client. For example:
	/// <list type="bullet">
	///		<item><description>Azure: <c>ServiceBusClient</c></description></item>
	///		<item><description>RabbitMQ: <c>IModel</c></description></item>
	///		<item><description>AWS: <c>AmazonSQSClient</c></description></item>
	/// </list>
	/// </typeparam>
	/// <param name="handler">A function that performs operations using the provider-specific client.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	/// <exception cref="ArgumentException">Thrown when an unsupported client type is specified.</exception>
	/// <remarks>
	/// <list type="bullet">
	///		<item><description>Azure: <c>ServiceBusClient</c></description></item>
	///		<item><description>RabbitMQ: <c>IModel</c></description></item>
	///		<item><description>AWS: <c>AmazonSQSClient</c></description></item>
	/// </list>
	/// </remarks>
	Task UseClient<T>(Func<T, Task> handler);

}