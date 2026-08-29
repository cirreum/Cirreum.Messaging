namespace Cirreum.Messaging.Tests;

/// <summary>
/// Proofs for the tuning-aware overloads on <see cref="IMessagingClient"/>. Both are default
/// interface methods, so an implementation that has not opted into tuning inherits them: empty
/// tuning must delegate to the untuned overload, and tuning carrying a value must fail loudly
/// rather than be discarded on the way to the broker.
/// <para>
/// Each test binds the client through <see cref="IMessagingClient"/> deliberately: a default
/// interface method is not a member of the implementing type, so the tuning overloads are
/// reachable only through the interface.
/// </para>
/// </summary>
public class MessagingClientTuningTests {

	[Fact]
	public void Empty_tuning_delegates_to_the_untuned_queue_receiver() {
		var recorder = new UntunedClient();
		IMessagingClient client = recorder;

		client.UseQueueReceiver("orders", new ReceiverTuning());

		recorder.RequestedQueue.Should().Be("orders");
	}

	[Fact]
	public void Tuning_that_carries_a_value_is_refused_by_a_client_that_has_not_opted_in() {
		var recorder = new UntunedClient();
		IMessagingClient client = recorder;

		var act = () => client.UseQueueReceiver("orders", new ReceiverTuning { PrefetchCount = 20 });

		act.Should().Throw<NotSupportedException>()
			.WithMessage("*does not support receiver tuning*");
		recorder.RequestedQueue.Should().BeNull();
	}

	[Fact]
	public void Empty_tuning_delegates_to_the_untuned_subscription() {
		var recorder = new UntunedClient();
		IMessagingClient client = recorder;

		client.UseSubscription("races", "results", new ReceiverTuning());

		recorder.RequestedSubscription.Should().Be("races/results");
	}

	[Fact]
	public void Tuned_subscription_is_refused_by_a_client_that_has_not_opted_in() {
		var recorder = new UntunedClient();
		IMessagingClient client = recorder;

		var act = () => client.UseSubscription("races", "results", new ReceiverTuning { PrefetchCount = 20 });

		act.Should().Throw<NotSupportedException>();
		recorder.RequestedSubscription.Should().BeNull();
	}

	[Fact]
	public void Null_tuning_is_rejected() {
		var recorder = new UntunedClient();
		IMessagingClient client = recorder;

		var queue = () => client.UseQueueReceiver("orders", null!);
		var subscription = () => client.UseSubscription("races", "results", null!);

		queue.Should().Throw<ArgumentNullException>();
		subscription.Should().Throw<ArgumentNullException>();
	}

	/// <summary>
	/// A client that implements only the untuned members, so both tuning overloads resolve to the
	/// interface's default implementations. Records what the defaults delegated to.
	/// </summary>
	private sealed class UntunedClient : IMessagingClient {

		public string? RequestedQueue { get; private set; }

		public string? RequestedSubscription { get; private set; }

		public IMessagingQueueReceiver UseQueueReceiver(string queue) {
			this.RequestedQueue = queue;
			return Substitute.For<IMessagingQueueReceiver>();
		}

		public IMessagingSubscriptionReceiver UseSubscription(string topic, string subscription) {
			this.RequestedSubscription = $"{topic}/{subscription}";
			return Substitute.For<IMessagingSubscriptionReceiver>();
		}

		public IMessagingQueue UseQueue(string queue) => throw new NotSupportedException();

		public IMessagingQueueSender UseQueueSender(string queue) => throw new NotSupportedException();

		public IMessagingTopicSender UseTopic(string topic) => throw new NotSupportedException();

		public Task UseClient<T>(Func<T, Task> handler) => throw new NotSupportedException();
	}
}
