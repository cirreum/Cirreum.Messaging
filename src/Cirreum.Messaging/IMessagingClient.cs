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