# Cirreum.Messaging Changelog

All notable changes to **Cirreum.Messaging** are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

For detailed migration steps on major version bumps, see the per-version migration
guides linked at the bottom of each entry.

---

## [Unreleased]

### Breaking

- **Each property bag has one owner, and is named for it.** `Properties` becomes
  `ApplicationProperties` on `IMessagingSendableMessage`, `IMessagingReceivableMessage` and
  `OutboundMessage`; `SystemProperties` is added as a required member, reserved for the framework
  and typed `IDictionary<string, string>` rather than `object`.

  `Properties` had three writers and no stated owner: the application's metadata, the distributed
  layer's routing keys, and whatever a provider chose to forward. That left the framework's
  self-echo skip resting on three assumptions the abstraction never made — that a provider forwards
  the property, that it does not filter it, and that the value returns with the type it was written
  with. The last was read through a type test that produces no match, no log and no error when it
  misses, and holds only because the one provider's header model happens to return a string as a
  string.

  System property values are strings so a provider's header model cannot alter the shape in
  transit, and keys carry a reserved `cirreum.` prefix — see `SystemProperty.IsReserved` and
  `OutboundMessage.WithSystemProperty`, which enforces it. The package does not define any system
  property key; the framework component that writes one owns it.

- **`OutboundMessage.StandardProperties` is removed.** It existed so a provider could share one
  definition of "already a first-class message field". No code referenced it — the only provider
  that needed the list declared its own copy, which had already drifted — and providers no longer
  filter application properties, so it has nothing left to serve.

- **Provider obligations are stated on the interfaces.** Application properties are forwarded as
  supplied, system properties are forwarded and never filtered, and a null-valued application
  property is not transmitted. These were previously per-provider liberties.

  See [MIGRATION-v2](MIGRATION-v2.md).


### Fixed

- **`OutboundMessage` copies the property bags it is given instead of adopting them.** The
  constructors taking `properties` / `providerProperties` assigned the caller's instance directly.
  A message is stamped with routing keys before it reaches the broker, so those writes landed in
  the caller's dictionary — and a read-only or frozen dictionary threw `NotSupportedException` at
  send time rather than at construction. Each bag is now copied, and a null bag is rejected by the
  constructor. A message's own bag uses ordinal key comparison regardless of how the supplied
  dictionary was built.

- **`ReceiverTuning.PrefetchCount` enforces the bound it documents.** A negative value was accepted
  and left for each broker implementation to reject; it now throws `ArgumentOutOfRangeException`
  where the value is set.

### Updated

- Adds the test project the repository did not have. It covers the property-bag ownership above, the
  `ReceiverTuning` bounds and the equality the receiver cache compares on, and the `IMessagingClient`
  tuning default-interface methods — empty tuning delegating to the untuned overload, and tuning
  carrying a value failing loudly.

## [1.1.1] - 2026-08-04

### Breaking

- **`ReceiverTuning.MaxAutoLockRenewalDuration` removed** — a same-day, pre-adoption
  correction to 1.1.0. Implementing the Azure mapping surfaced that the pull-based receiver
  surface has no creation-time renewal option to map it onto (auto-renewal is a push-processor
  feature): the knob could never have a consumer, which is the inert-configuration defect this
  chain exists to eliminate. Lock renewal is a processing-time behavior over the existing
  `IMessagingReceivedMessage.RenewLockAsync`, owned by whoever runs the handler — the
  distributed channel's receive loop honors `ReceiverOptions.MaxAutoLockRenewalDuration` there
  (see `Cirreum.Runtime.Messaging`). `ReceiverTuning` is creation-time broker options only:
  `PrefetchCount`. Shipped as a patch deliberately (`-AllowBreakingPatch`): 1.1.0 released
  earlier today with zero adopters.

## [1.1.0] - 2026-08-04

### Added

- **`ReceiverTuning`** — broker-agnostic, creation-time receiver tuning (`PrefetchCount`,
  `MaxAutoLockRenewalDuration`); every value nullable, null meaning "the broker's own
  default". (From the backlog: head of the three-repo receiver-tuning chain.)
- **`UseQueueReceiver(string, ReceiverTuning)` and `UseSubscription(string, string,
  ReceiverTuning)`** — default-implemented overloads on `IMessagingClient`. The defaults
  delegate to the untuned methods when the tuning carries no values and throw
  `NotSupportedException` when it does, so an implementation that has not opted into tuning
  fails loudly instead of silently discarding it — the discard-is-invisible failure this
  chain exists to eliminate. Implementations that cache receivers by name must throw when
  the same receiver is requested with different tuning; the contract documents this.

## [1.0.109] - 2026-07-05

### Fixed

- README described the distributed-messaging layer (envelopes, registry, batching) with never-shipped APIs — this package is the broker abstraction only (`IMessagingClient`, queues/topics/senders/receivers); the distributed channel lives in `Cirreum.Messaging.Distributed`. Rewritten to describe the actual shipped surface. No effect on the package itself; this release exists to re-trigger a working publish run past the impact-check gate.
