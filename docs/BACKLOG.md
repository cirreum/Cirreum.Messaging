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

### Framework-written message properties have no declared value shape

**SemVer:** Minor
**Trigger:** A second messaging provider lands (AWS SNS/SQS or Kafka), or any framework
component starts branching on a `Properties` value beyond a presence check.
**Noted:** 2026-08-29

`IMessagingReceivableMessage.Properties` and `ProviderProperties` are
`IReadOnlyDictionary<string, object>`. That is the right type for a broker property bag —
AMQP alone carries strings, integers, booleans and timestamps, and the value's shape is the
provider's to decide, not ours.

It is the wrong type for the properties the *framework itself* writes and reads back. The
delivery engine stamps its own routing keys onto every outbound message (identifier, version,
producer, node), and the receiver reads the node marker back as:

```csharp
if (received.Properties.TryGetValue(Property_Node, out var nodeObj)
    && nodeObj is string remoteNode
    && remoteNode == this._nodeIdProvider.NodeId) {
```

A value that is not a `string` fails the pattern, the self-echo skip does not happen, and the
node reprocesses its own message — with no log line and no failed assertion. The behaviour is
correct today because Azure Service Bus is the only provider and its `ApplicationProperties`
round-trip a string as a string. It is not correct by contract: the delivery engine's own
documentation anticipates SNS message attributes and Kafka headers, neither of which surfaces
a `string`. Kafka headers are `byte[]`; SNS attributes are typed wrappers. The first such
provider turns a silent no-op into duplicate processing.

The shape of the defect is a value whose runtime type is decided by infrastructure
configuration rather than by the contract, read through a type test that degrades silently
when it misses. `OperationGrant.Extensions` was the same shape with a different mechanism, and
was closed by typing the value (Cirreum.Contracts 6.0.0).

Two candidate fixes, in preference order:

1. State the contract for framework-written properties — a provider must surface a value the
   framework wrote as the type it wrote — and give the provider abstraction a normalizing
   accessor (`TryGetString(key, out value)` or similar) that every provider implements
   against its own header model. Reading through the accessor removes the type test from
   call sites entirely.
2. Leave the dictionary alone and make the miss observable: log at warning when a
   framework-written property is present but not the expected shape. Cheaper, and it converts
   a silent duplicate-processing bug into a diagnosable one, but it still leaves each call
   site to get the test right.

The symptom is felt in `Cirreum.Runtime.Messaging`; the ambiguity is declared here, so the
work starts here.
