# Backlog

Deferred work for **Cirreum.Messaging**. Items here are tracked but not yet ready
to ship — either because the cost outweighs the benefit in isolation, or
because they're waiting on a forcing function (a related change, a consumer
upgrade, a coordinated multi-repo rollout).

## How this file works

- Each item is a `###` heading so it can be linked to and parsed.
- Each item declares **`SemVer:`** (`Patch` | `Minor` | `Major` | `Unspecified`),
  **`Trigger:`** (the human-readable condition that will make it ready), and
  **`Noted:`** (the date the item was added).
- The Cirreum DevOps release scripts (`PatchRelease`, `MinorRelease`,
  `MajorRelease`) surface items at-or-below the requested bump level so the
  operator can decide whether to fold them in before tagging.
- Items that ship: move from this file to `docs/CHANGELOG.md` under
  `[Unreleased]`. Items that grow into design discussions: promote to an ADR.

## Queued

### Grow the receiver-creation API with broker tuning options

- **SemVer:** Minor
- **Trigger:** None — nothing blocks this. It is the **root of a three-repo chain** and must ship first; do it on the next minor cut here.
- **Noted:** 2026-07-27

**This is the head of the chain. Start here.**

`IMessagingClient` creates receivers with no tuning parameters:

```csharp
IMessagingQueueReceiver UseQueueReceiver(string queue);
IMessagingSubscriptionReceiver UseSubscription(string topic, string subscription);
```

Because there is nowhere to pass them, `ReceiverOptions.PrefetchCount` and
`ReceiverOptions.MaxAutoLockRenewalDuration` in `Cirreum.Messaging.Distributed` are **inert** — bindable,
documented, defaulted configuration that reaches no broker. `PrefetchCount` is worse than absent: it
documents a default of `10` while the SDK's own default silently applies instead. An operator tuning
prefetch for throughput gets no error and no effect.

**Design constraint — do not take a dependency on `Cirreum.Messaging.Distributed`.** That package is a
**peer in Common**, and the layer model forbids same-layer dependencies. This package is also
deliberately dependency-free (zero Cirreum references), and `ReceiverOptions` is a *channel* concept,
not a *broker* one. The fix is a broker-level tuning type owned here — an optional parameter or a
configure callback on the two factory methods — which `Cirreum.Runtime.Messaging` maps its
`ReceiverOptions` onto at the point of use.

Additive at this layer: keep the existing signatures working (optional parameter or overload), so no
consumer is forced to change.

**The rest of the chain, in release order:**

| Repo | Layer | Work |
|---|---|---|
| **`Cirreum.Messaging`** (here) | Common | grow the contract — **blocks the other two** |
| `Cirreum.Messaging.Azure` | Infrastructure | map the values onto `ServiceBusReceiverOptions` / `ServiceBusProcessorOptions` |
| `Cirreum.Runtime.Messaging` | Runtime Extensions | pass `ReceiverOptions` through `DistributedMessageReceiver` |

Both downstream repos carry a mirror of this item marked **`SemVer: Major`** — deliberately, so a
cross-repo item that cannot start until this contract moves does not surface as noise on their every
patch and minor. This one is `Minor` precisely because it is the root: it should surface early, while
acting on it is still cheap.

> **Why this item exists here at all.** It was originally filed only in `Cirreum.Runtime.Messaging`,
> the repo where the *symptom* lives — which is the last rung released. It therefore surfaced only
> after Common and Infrastructure had already shipped and closed, when folding it in would have meant
> reopening two rungs from the bottom. **File a cross-repo item in the repo where the work starts, not
> only where it is felt.**
