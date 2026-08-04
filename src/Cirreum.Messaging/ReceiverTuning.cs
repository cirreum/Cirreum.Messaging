namespace Cirreum.Messaging;

/// <summary>
/// Broker-level tuning applied when a receiver is created. Broker-agnostic — each
/// <see cref="IMessagingClient"/> implementation maps the values onto its own SDK's
/// receiver-creation options.
/// </summary>
/// <remarks>
/// <para>
/// Every value is nullable, and <see langword="null"/> means "use the broker's own default".
/// An instance carrying no explicit values (<see cref="IsEmpty"/>) is equivalent to not
/// supplying tuning at all.
/// </para>
/// <para>
/// This is a <em>creation-time</em> concept: receivers are cached by name, so tuning
/// participates in the first creation only. Implementations must throw when the same
/// receiver is requested again with different tuning rather than silently returning the
/// previously created instance — a tuning value that binds but never reaches the broker
/// is indistinguishable from working.
/// </para>
/// </remarks>
public sealed record ReceiverTuning {

	/// <summary>
	/// Number of messages the broker client prefetches into its local buffer ahead of
	/// processing. Higher values increase throughput at the cost of longer lock exposure
	/// for buffered messages. <c>0</c> disables prefetch. Must not be negative.
	/// </summary>
	public int? PrefetchCount { get; init; }

	/// <summary>
	/// How long the broker client keeps auto-renewing a received message's lock while it
	/// is being processed. <see cref="TimeSpan.Zero"/> disables auto-renewal.
	/// </summary>
	public TimeSpan? MaxAutoLockRenewalDuration { get; init; }

	/// <summary>
	/// Whether this instance carries no explicit values — equivalent to supplying no
	/// tuning at all.
	/// </summary>
	public bool IsEmpty => this.PrefetchCount is null && this.MaxAutoLockRenewalDuration is null;

}
