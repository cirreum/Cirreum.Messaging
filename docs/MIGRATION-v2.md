# Cirreum.Messaging v1 → v2 Migration

v2 gives each message property bag one owner, and names it for that owner.

| v1 | v2 | Owner |
| --- | --- | --- |
| `Properties` | `ApplicationProperties` | the application |
| — | `SystemProperties` | Cirreum |
| `ProviderProperties` | `ProviderProperties` | the messaging provider |

For an application the change is a rename. Nothing an application writes belongs in
`SystemProperties`.

---

## Rename

```csharp
// v1
message.Properties["tenant"] = tenantId;
var tenant = received.Properties["tenant"];

// v2
message.ApplicationProperties["tenant"] = tenantId;
var tenant = received.ApplicationProperties["tenant"];
```

The sender overloads keep their shape; only the parameter name changed:

```csharp
Task PublishMessageAsync(string message, IDictionary<string, object> applicationProperties, CancellationToken ct = default);
Task PublishMessagesAsync(IEnumerable<OutboundMessage> messages, IDictionary<string, object>? commonApplicationProperties = null, CancellationToken ct = default);
```

Named arguments (`properties:`, `commonProperties:`) need updating; positional calls do not.

## `SystemProperties`

New, reserved for the framework, and typed `string` rather than `object` so a provider's header
model cannot change the value's shape in transit. Keys carry the `cirreum.` prefix; write through
the fluent helper, which enforces it:

```csharp
outboundMessage.WithSystemProperty("cirreum.node", nodeId);
```

An unreserved key throws `ArgumentException`. Use `SystemProperty.IsReserved(key)` to test a key.

Applications do not write system properties. They may read them from a received message —
`received.SystemProperties` — but the keys belong to whichever framework component set them.

## `StandardProperties` is removed

`OutboundMessage.StandardProperties` listed the property names that map to first-class message
fields, so a provider could avoid duplicating them. No code referenced it: the one provider that
needed the list declared its own copy, which had already drifted. Providers no longer filter
application properties, so the list has nothing left to serve.

If you referenced it, you were reimplementing a provider-side filter that no longer exists; delete
the code that consumed it.

## Behaviour changes worth knowing

**An application property is no longer dropped for colliding with a message field name.** Under v1
the Azure provider silently discarded any property keyed `MessageId`, `CorrelationId`,
`ContentType`, `Subject`, `TimeToLive`, or `ReplyTo`. Those now travel. Application properties
occupy a namespace separate from those fields on the wire, so there was never a collision to
prevent.

**A property whose key is reserved is not forwarded from the application bag.** Writing
`ApplicationProperties["cirreum.node"]` has no effect on the wire; the reserved space belongs to the
framework.

**A null-valued application property is still not transmitted**, and this is now stated on the
interface rather than left to each provider.

## Implementing `IMessagingSendableMessage` or `IMessagingReceivableMessage`

Both interfaces gained `SystemProperties` as a required member — deliberately not a default
implementation, because a provider that silently returned an empty bag would drop framework metadata
without any signal, which is the failure this release removes.

An implementation must forward system properties exactly as set, and never filter one. Where a
broker exposes a single property dictionary on the wire, merge the bags on send and split them on
receive by `SystemProperty.IsReserved`. Read a system value through `ToString()` rather than a type
test, so a broker returning another representation cannot produce a silent miss.
