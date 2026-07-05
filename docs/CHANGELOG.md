# Cirreum.Messaging Changelog

All notable changes to **Cirreum.Messaging** are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

For detailed migration steps on major version bumps, see the per-version migration
guides linked at the bottom of each entry.

---

## [Unreleased]

### Fixed

- README described the distributed-messaging layer (envelopes, registry, batching) with never-shipped APIs — this package is the broker abstraction only (`IMessagingClient`, queues/topics/senders/receivers); the distributed channel lives in `Cirreum.Messaging.Distributed`. Rewritten to describe the actual shipped surface. No effect on the package itself; this release exists to re-trigger a working publish run past the impact-check gate.
