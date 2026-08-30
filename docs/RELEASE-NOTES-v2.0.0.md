# Cirreum.Messaging 2.0.0

Each message property bag now has exactly one owner, and is named for it.

| Bag | Owner | Value type |
| --- | --- | --- |
| `ApplicationProperties` (was `Properties`) | the application | `object` |
| `SystemProperties` (new) | Cirreum | `string` |
| `ProviderProperties` | the messaging provider | `object` |

## Why

`Properties` had three writers and no stated owner. The application put its metadata there, the
distributed layer used it as a wire protocol — stamping four `cirreum.*` routing keys onto every
outbound message and reading one of them back to drop its own echo — and the provider treated the
whole bag as advisory, silently discarding any entry whose key collided with a first-class message
field or whose value was null.

That left the framework's own self-echo skip resting on three assumptions the abstraction never
made: that a provider forwards the property, that it does not filter it, and that the value returns
with the type it was written with. The last one was read as `nodeObj is string remoteNode`, which
produces no match, no log, and no error when it misses. It holds today only because Azure Service
Bus is the sole provider and its AMQP mapping happens to return a string as a string.

Splitting the bags puts each assumption on a stated contract instead. `SystemProperties` is reserved
for the framework and typed `string`, so the round trip cannot change the shape and the reader needs
no type test at all.

## What changes for an application

A rename: `message.Properties` becomes `message.ApplicationProperties`. Nothing an application
writes belongs in `SystemProperties`.

Two behaviours change. An application property keyed like a message field (`Subject`, `MessageId`,
and the rest) is now carried rather than silently dropped. A property whose key falls in the
reserved `cirreum.` space is not forwarded from the application bag.

`OutboundMessage.StandardProperties` is removed; nothing referenced it.

## Also in this release

`OutboundMessage` now copies the property dictionaries it is given rather than adopting the caller's
instance, so framework stamps can no longer escape into the caller's dictionary and a read-only
dictionary can be supplied. `ReceiverTuning.PrefetchCount` enforces the non-negative bound it
documents. The package has a test project for the first time.

Full steps in [MIGRATION-v2](MIGRATION-v2.md).
