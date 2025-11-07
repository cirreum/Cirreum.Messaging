namespace Cirreum.Messaging;

public interface IMessagingQueueReceivedMessage : IMessagingReceivedMessage {
	/// <summary>
	/// Gets the name of the queue from which this message was received.
	/// </summary>
	string Queue { get; }
}