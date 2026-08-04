# Cirreum.Messaging 1.1.0 — receiver tuning reaches the broker contract

## Why this release exists

`ReceiverOptions` in `Cirreum.Messaging.Distributed` has carried `PrefetchCount` and
`MaxAutoLockRenewalDuration` since it shipped — bindable, documented, defaulted — and neither
value has ever reached a broker. The factory methods on `IMessagingClient` took no tuning
parameters, so there was nowhere to pass them. `PrefetchCount` was the worst case: it documents
a default of 10 while the SDK's own default silently applied instead. An operator tuning
prefetch for throughput got no error and no effect.

This release is the head of the three-repo chain that fixes that. It grows the broker contract;
the provider mapping (`Cirreum.Messaging.Azure`) and the channel flow-through
(`Cirreum.Runtime.Messaging`) follow in their own releases.

## What's new

**`ReceiverTuning`** — a broker-agnostic, creation-time tuning record. Every value is nullable,
and null means "the broker's own default":

```csharp
var receiver = client.UseQueueReceiver("orders.pending.v1", new ReceiverTuning {
	PrefetchCount = 50,
	MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(10)
});
```

**Overloads on `IMessagingClient`** — `UseQueueReceiver(string, ReceiverTuning)` and
`UseSubscription(string, string, ReceiverTuning)`, both default-implemented so existing
`IMessagingClient` implementations keep compiling:

- When the tuning carries **no values**, the default implementations delegate to the untuned
  methods — supplying an empty `ReceiverTuning` is exactly equivalent to supplying none.
- When the tuning carries **values**, the default implementations throw
  `NotSupportedException`. An implementation that has not opted into tuning fails loudly at
  the call site instead of silently discarding the values — the discard-is-invisible failure
  this chain exists to eliminate.

**A cache-honesty rule, stated on the contract.** Receivers are cached by name, so tuning
participates in the first creation only. Implementations must throw when the same receiver is
requested again with *different* tuning rather than silently returning the previously created
instance.

## Coordinated downstream work

| Repo | Layer | Work |
|---|---|---|
| `Cirreum.Messaging` (this release) | Common | the contract |
| `Cirreum.Messaging.Azure` | Infrastructure | map the values onto Service Bus receiver creation |
| `Cirreum.Runtime.Messaging` | Runtime Extensions | flow `ReceiverOptions` values through `DistributedMessageReceiver` |

Until all three ship, the two `ReceiverOptions` knobs remain inert — but tuning passed directly
to a provider that has shipped its mapping works as soon as that provider does.

## Compatibility

Fully additive. Existing implementations of `IMessagingClient` compile unchanged; existing
callers are unaffected. The only new runtime behavior is deliberate: passing value-carrying
tuning to a provider that does not support it throws `NotSupportedException` instead of doing
nothing.

## See also

- `docs/CHANGELOG.md` — the enumerated changes
- `Cirreum.Messaging.Distributed` — `ReceiverOptions`, the channel-level configuration these
  values originate from
