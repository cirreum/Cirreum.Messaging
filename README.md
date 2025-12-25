# Cirreum Messaging Library

[![NuGet Version](https://img.shields.io/nuget/v/Cirreum.Messaging.svg?style=flat-square)](https://www.nuget.org/packages/Cirreum.Messaging/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Cirreum.Messaging.svg?style=flat-square)](https://www.nuget.org/packages/Cirreum.Messaging/)
[![GitHub Release](https://img.shields.io/github/v/release/cirreum/Cirreum.Messaging?style=flat-square)](https://github.com/cirreum/Cirreum.Messaging/releases)

A comprehensive .NET library for building distributed messaging systems with versioning, background processing, and transport abstraction. The library provides a clean, provider-agnostic API for working with message queues and topics while supporting schema evolution and backward compatibility

## Features

### 🚀 **Core Messaging Capabilities**

- **Queue and Topic Support**: Send messages to queues (single consumer) or topics (multiple subscribers)
- **Transport Abstraction**: Provider-agnostic interface supporting Azure Service Bus, AWS SQS/SNS, RabbitMQ, and more
- **Async/Await**: Fully asynchronous API with cancellation token support
- **Batch Operations**: Efficient batch sending and receiving of messages

### 📦 **Message Versioning & Schema Evolution**

- **Versioned Messages**: Built-in support for message schema versioning
- **Backward Compatibility**: Handle multiple message versions simultaneously
- **Schema Definitions**: Stable message identities that persist across versions
- **Type Safety**: Strong typing with compile-time safety

### ⚡ **High-Performance Processing**

- **Background Delivery**: Optional background processing with configurable batching
- **Throughput Optimization**: Configurable batch sizes and timeouts
- **Memory Efficient**: Bounded channels with configurable capacity
- **Delivery Guarantees**: Choose between fire-and-forget or confirmed delivery

### 🔧 **Developer Experience**

- **Conductor Integration**: Seamless integration with Conductor for CQRS patterns
- **Fluent API**: Clean, discoverable API design
- **Configuration-Driven**: Extensive configuration options via appsettings.json
- **Testing Support**: Empty transport publisher for development and testing

## Quick Start

### 1. Define Your Messages

```csharp
// Define message categories
public static class UserMessages 
{
    public static MessageDefinition Created => 
        Messages.Define("User", nameof(Created));
}

// Create versioned messages
public record UserCreatedV1(string Username, string Email) : ITopicMessage 
{
    public MessageDefinition Definition { get; } = UserMessages.Created;
    public int Version { get; } = 1;
}

public record UserCreatedV2(string Username, string Email, string DisplayName) : ITopicMessage 
{
    public MessageDefinition Definition { get; } = UserMessages.Created;
    public int Version { get; } = 2;
}
```

### 2. Configure Services

```csharp
// In Program.cs or Startup.cs
services.Configure<DistributionOptions>(configuration.GetSection("Distribution"));
services.AddScoped<IDistributedTransportPublisher, DefaultTransportPublisher>();

// Register Conductor for automatic message distribution
services.AddConductor();
services.AddScoped(typeof(INotificationHandler<>), typeof(DistributedMessageHandler<>));
```

### 3. Configuration

```json
{
  "Distribution": {
    "Sender": {
      "InstanceKey": "app-primary",
      "QueueName": "app.commands.v1",
      "TopicName": "app.events.v1",
      "UseBackgroundDelivery": true,
      "BackgroundDelivery": {
        "QueueCapacity": 1000,
        "BatchSize": 10,
        "BatchingInterval": "00:00:00.050",
        "WaitForCompletion": false,
        "CompletionTimeout": "00:00:10"
      }
    }
  }
}
```

### 4. Send Messages

```csharp
// Via Conductor (recommended)
await conductor.PublishAsync(new UserCreatedV2("john_doe", "john@example.com", "John Doe"));

// Direct via transport publisher
await transportPublisher.PublishMessageAsync(
    new UserCreatedV2("jane_doe", "jane@example.com", "Jane Doe"), 
    cancellationToken);
```

### 5. Receive Messages

```csharp
// Azure Functions example
[Function("ProcessUserEvents")]
public async Task ProcessUserEvents(
    [ServiceBusTrigger("app.events.v1", "user-processor")] 
    ServiceBusReceivedMessage message)
{
    var envelope = DistributedMessageEnvelope.FromJson(message.Body.ToString());
    
    if (envelope.MessageDefinition == UserMessages.Created)
    {
        switch (envelope.MessageVersion)
        {
            case 1:
                var v1 = envelope.DeserializeMessage<UserCreatedV1>();
                await ProcessUserCreatedV1(v1);
                break;
            case 2:
                var v2 = envelope.DeserializeMessage<UserCreatedV2>();
                await ProcessUserCreatedV2(v2);
                break;
        }
    }
}
```

## Architecture

### Message Flow

```text
Application Code
      ↓
   Conductor PublishAsync
      ↓
DistributedMessageHandler
      ↓
IDistributedTransportPublisher
      ↓
Message Transport (Azure/AWS/RabbitMQ)
      ↓
Consumer Applications
```

### Key Components

- **`IDistributedMessage`**: Base interface for all distributed messages
- **`ITopicMessage`/`IQueueMessage`**: Specialized interfaces for different destination types
- **`MessageDefinition`**: Stable message identity across versions
- **`DistributedMessageEnvelope`**: Serialization wrapper with metadata
- **`IDistributedTransportPublisher`**: Transport abstraction layer
- **`DefaultTransportPublisher`**: Production implementation with background processing

## Message Versioning Best Practices

### Schema Evolution

When evolving message schemas:

1. **Never modify existing message classes** once deployed to production
2. **Create new versions** with incremented version numbers
3. **Keep the same MessageDefinition** across versions
4. **Document changes** between versions

```csharp
// ✅ Good: New version with same definition
public record UserCreatedV3(
    string Username, 
    string Email, 
    string DisplayName, 
    bool IsVerified,
    DateTime CreatedAt) : ITopicMessage 
{
    public MessageDefinition Definition { get; } = UserMessages.Created; // Same definition
    public int Version { get; } = 3; // Incremented version
}

// ❌ Bad: Modifying existing message
public record UserCreatedV2(
    string Username, 
    string Email, 
    string DisplayName,
    bool IsVerified) // Don't add fields to existing versions!
```

### Handling Multiple Versions

```csharp
// Consumer can handle multiple versions
switch (envelope.MessageVersion)
{
    case 1:
        var v1 = envelope.DeserializeMessage<UserCreatedV1>();
        await ProcessUser(v1.Username, v1.Email, displayName: null);
        break;
    case 2:
        var v2 = envelope.DeserializeMessage<UserCreatedV2>();
        await ProcessUser(v2.Username, v2.Email, v2.DisplayName);
        break;
    case 3:
        var v3 = envelope.DeserializeMessage<UserCreatedV3>();
        await ProcessUser(v3.Username, v3.Email, v3.DisplayName, v3.IsVerified);
        break;
}
```

## Advanced Configuration

### Background Delivery Options

```csharp
public class BackgroundDeliveryOptions 
{
    public int QueueCapacity { get; set; } = 1000;      // Max queued messages
    public int BatchSize { get; set; } = 10;            // Messages per batch
    public TimeSpan BatchingInterval { get; set; }      // Max wait for batch
    public bool WaitForCompletion { get; set; }         // Fire-and-forget vs confirmed
    public TimeSpan CompletionTimeout { get; set; }     // Timeout for confirmation
}
```

### Per-Message Overrides

```csharp
public record CriticalAlert(string Message) : ITopicMessage 
{
    public MessageDefinition Definition { get; } = AlertMessages.Critical;
    public int Version { get; } = 1;
    
    // Override global settings for this message type
    public bool? UseBackgroundDelivery => false;  // Force synchronous delivery
    public bool? WaitForCompletion => true;       // Ensure delivery confirmation
}
```

## Testing

Use the `EmptyTransportPublisher` for development and testing:

```csharp
// In test configuration
services.AddScoped<IDistributedTransportPublisher, EmptyTransportPublisher>();

// Messages will be logged but not actually sent
await transportPublisher.PublishMessageAsync(testMessage, cancellationToken);
// Logs: "EmptyTransportPublisher received Message UserCreatedV2 - not published"
```

## Transport Providers

The library supports multiple messaging providers through the `IMessagingClient` abstraction:

- **Azure Service Bus**: Topics, queues, and subscriptions
- **AWS SQS/SNS**: Queues and topics
- **RabbitMQ**: Exchanges and queues
- **In-Memory**: For testing and development

Each provider implements the common interfaces while exposing provider-specific features through the `UseClient<T>()` method.

## Contributing

This package is part of the Cirreum ecosystem. Follow the established patterns when contributing new features or provider implementations.
