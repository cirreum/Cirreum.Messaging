# Cirreum.Messaging Changelog

All notable changes to **Cirreum.Messaging** are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

For detailed migration steps on major version bumps, see the per-version migration
guides linked at the bottom of each entry.

---

## [Unreleased]

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
