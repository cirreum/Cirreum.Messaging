# Cirreum.Messaging

[![NuGet Version](https://img.shields.io/nuget/v/Cirreum.Messaging.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Messaging/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Cirreum.Messaging.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Messaging/)
[![GitHub Release](https://img.shields.io/github/v/release/cirreum/Cirreum.Messaging?style=flat-square&labelColor=1F1F1F&color=FF3B2E)](https://github.com/cirreum/Cirreum.Messaging/releases)
[![License](https://img.shields.io/badge/license-MIT-F2F2F2?style=flat-square&labelColor=1F1F1F)](https://github.com/cirreum/Cirreum.Messaging/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-003D8F?style=flat-square&labelColor=1F1F1F)](https://dotnet.microsoft.com/)

**Provider-agnostic broker abstractions for the Cirreum Messaging track — queues, topics, subscriptions, and the outbound/received message model.**

## Overview

**Cirreum.Messaging** defines the transport abstraction the rest of the Messaging track builds on: a clean, broker-neutral API for sending to and receiving from queues, topics, and subscriptions. Provider packages (e.g., `Cirreum.Messaging.Azure` for Azure Service Bus) implement these interfaces; consumers program against the abstractions and never touch a broker SDK directly.

Cirreum.Messaging contains:

- **`IMessagingClient`** — the entry point: `UseQueue` / `UseQueueSender` / `UseQueueReceiver` / `UseTopic` / `UseSubscription`, plus `UseClient<T>` to escape-hatch to the native provider client
- **Sending** — `IMessagingQueueSender` / `IMessagingTopicSender` with single and batch operations, and `OutboundMessage` (content, subject, correlation, TTL, and the property bags below)
- **Receiving** — `IMessagingQueueReceiver` / `IMessagingSubscriptionReceiver` with receive-one, receive-many, streaming (`ReceiveMessagesStreamAsync`), and peek; received messages expose content, properties, and the broker ack model (`Complete` / `Abandon` / `Defer` / `DeadLetter` / `RenewLock`)
- **Property bags** — three, each with one owner: `ApplicationProperties` for the application's own cross-broker filterable metadata, `SystemProperties` for framework metadata a receiver reads without deserializing the payload (reserved keys, `string` values), and `ProviderProperties` for provider-specific knobs. A provider forwards the first two as supplied and never filters a system property.
- **`ReceiverTuning`** — optional broker-level tuning applied when a receiver is created (`PrefetchCount`), via overloads of `UseQueueReceiver` / `UseSubscription`

## Quick Start

Register a provider instance (see the provider package for configuration-driven registration; instances are **keyed** `IMessagingClient` registrations):

```csharp
builder.AddAzureMessagingClient("primary", connectionString); // Cirreum.Messaging.Azure
```

Send and receive against the abstractions:

```csharp
public sealed class OrderQueueService(
	[FromKeyedServices("primary")] IMessagingClient client) {

	public Task SendAsync(Order order, CancellationToken ct) =>
		client.UseQueueSender("orders.pending.v1")
			.PublishMessageAsync(
				OutboundMessage.AsJsonContent(order).WithSubject("orders.created"),
				ct);

	public async Task ProcessOneAsync(CancellationToken ct) {
		var received = await client.UseQueueReceiver("orders.pending.v1")
			.ReceiveMessageAsync(cancellationToken: ct);
		var order = JsonSerializer.Deserialize<Order>(received.ContentString);
		// ... handle ...
		await received.CompleteMessageAsync(ct); // or Abandon / Defer / DeadLetter
	}
}
```

Streaming consumption (competing consumers):

```csharp
await foreach (var received in client.UseQueueReceiver("orders.pending.v1")
	.ReceiveMessagesStreamAsync(stoppingToken)) {
	// ... handle + ack ...
}
```

Topics fan out to subscriptions:

```csharp
await client.UseTopic("app.notifications.v1")
	.BroadcastMessageAsync(OutboundMessage.AsJsonContent(notice).WithSubject("notice.raised"));

var received = await client.UseSubscription("app.notifications.v1", "api-head")
	.ReceiveMessageAsync();
```

## Receiver tuning

Receivers accept optional broker-level tuning at creation. Values you don't set fall back to
the broker's own defaults:

```csharp
var receiver = client.UseQueueReceiver("orders.pending.v1", new ReceiverTuning {
	PrefetchCount = 50
});
```

Message-lock renewal is deliberately **not** here: it is a processing-time behavior over
`IMessagingReceivedMessage.RenewLockAsync`, owned by whoever runs the handler (e.g. the
distributed channel's receive loop in `Cirreum.Runtime.Messaging`), not a receiver-creation
option.

Two rules keep tuning honest:

- **Provider support is required.** A provider that has not opted into tuning throws
  `NotSupportedException` when the tuning carries values — it never silently discards them.
- **Tuning is creation-time.** Receivers are cached by name, so requesting the same receiver
  again with *different* tuning throws rather than silently returning the previously created
  instance.

## Where distributed messaging lives

This package is deliberately transport-only. The versioned-envelope model (`DistributedMessage`, `DistributedMessageEnvelope`, the registry, batching policies) ships in **`Cirreum.Messaging.Distributed`**, and the runtime delivery engine (publish-through-Conductor, batching, the inbound receiver) ships in **`Cirreum.Runtime.Messaging`**. The two are peers of this package's abstractions — both compose on top of `IMessagingClient`.

## Contribution Guidelines

1. **Be conservative with new abstractions**  
   The API surface must remain stable and meaningful.

2. **Limit dependency expansion**  
   Only add foundational, version-stable dependencies.

3. **Favor additive, non-breaking changes**  
   Breaking changes ripple through the entire ecosystem.

4. **Include thorough unit tests**  
   All primitives and patterns should be independently testable.

5. **Document architectural decisions**  
   Context and reasoning should be clear for future maintainers.

6. **Follow .NET conventions**  
   Use established patterns from Microsoft.Extensions.* libraries.

## Versioning

Cirreum.Messaging follows [Semantic Versioning](https://semver.org/):

- **Major** - Breaking API changes
- **Minor** - New features, backward compatible
- **Patch** - Bug fixes, backward compatible

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Cirreum Foundation Framework**  
*Layered simplicity for modern .NET*
