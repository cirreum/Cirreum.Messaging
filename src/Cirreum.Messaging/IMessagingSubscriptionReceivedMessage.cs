namespace Cirreum.Messaging;

public interface IMessagingSubscriptionReceivedMessage : IMessagingReceivedMessage {
	/// <summary>
	/// Gets the name of the topic from which this message was received.
	/// </summary>
	string Topic { get; }
	/// <summary>
	/// Gets the name of the subscription from which this message was received.
	/// </summary>
	string Subscription { get; }
}