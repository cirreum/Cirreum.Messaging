namespace Cirreum.Messaging.Tests;

using System.Collections.ObjectModel;

/// <summary>
/// Proofs that an <see cref="OutboundMessage"/> owns its property bags, and that the framework's
/// bag is reserved. A message is stamped with framework-written routing keys before it reaches the
/// broker, so a bag shared with the caller would let those writes escape into the caller's
/// dictionary — or fail outright against one that does not support writes.
/// </summary>
public class OutboundMessageTests {

	private static Dictionary<string, object> Properties() =>
		new() { ["tenant"] = "acme" };

	[Fact]
	public void Supplied_properties_are_present_on_the_message() {
		var message = new OutboundMessage("payload", Properties());

		message.ApplicationProperties.Should().ContainKey("tenant").WhoseValue.Should().Be("acme");
	}

	[Fact]
	public void Mutating_the_supplied_dictionary_does_not_reach_the_message() {
		var supplied = Properties();
		var message = new OutboundMessage("payload", supplied);

		supplied["tenant"] = "changed";
		supplied["added-later"] = true;

		message.ApplicationProperties["tenant"].Should().Be("acme");
		message.ApplicationProperties.Should().NotContainKey("added-later");
	}

	[Fact]
	public void Stamping_the_message_does_not_reach_the_supplied_dictionary() {
		var supplied = Properties();
		var message = new OutboundMessage("payload", supplied);

		// What the delivery engine does to every outbound message before it is sent.
		message.ApplicationProperties["Cirreum.Node"] = "node-1";

		supplied.Should().NotContainKey("Cirreum.Node");
		supplied.Should().HaveCount(1);
	}

	[Fact]
	public void A_read_only_dictionary_can_be_supplied_and_the_message_stays_writable() {
		var supplied = new ReadOnlyDictionary<string, object>(Properties());

		var message = new OutboundMessage("payload", supplied);
		var stamp = () => message.ApplicationProperties["Cirreum.Node"] = "node-1";

		stamp.Should().NotThrow();
		message.ApplicationProperties["tenant"].Should().Be("acme");
	}

	[Fact]
	public void Provider_properties_are_copied_as_well() {
		var properties = Properties();
		var providerProperties = new Dictionary<string, object> { ["PartitionKey"] = "p1" };

		var message = new OutboundMessage("payload", properties, providerProperties);
		providerProperties["PartitionKey"] = "p2";
		message.ProviderProperties["Extra"] = true;

		message.ProviderProperties["PartitionKey"].Should().Be("p1");
		providerProperties.Should().NotContainKey("Extra");
	}

	[Fact]
	public void Null_property_bags_are_rejected() {
		var withNullProperties = () => new OutboundMessage("payload", null!);
		var withNullProviderProperties = () => new OutboundMessage("payload", Properties(), null!);

		withNullProperties.Should().Throw<ArgumentNullException>();
		withNullProviderProperties.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void A_message_built_without_properties_still_accepts_stamps() {
		var message = new OutboundMessage("payload");

		message.ApplicationProperties["Cirreum.Node"] = "node-1";

		message.ApplicationProperties.Should().ContainKey("Cirreum.Node");
	}

	// System properties ————————————————————————————————————————

	[Fact]
	public void A_reserved_system_property_is_accepted() {
		var message = new OutboundMessage("payload")
			.WithSystemProperty("cirreum.node", "node-1");

		message.SystemProperties["cirreum.node"].Should().Be("node-1");
	}

	[Fact]
	public void An_unreserved_system_property_key_is_rejected() {
		var message = new OutboundMessage("payload");

		var act = () => message.WithSystemProperty("node", "node-1");

		act.Should().Throw<ArgumentException>().WithMessage("*reserved*");
	}

	[Fact]
	public void System_properties_are_separate_from_application_properties() {
		var message = new OutboundMessage("payload", Properties())
			.WithSystemProperty("cirreum.node", "node-1");

		message.ApplicationProperties.Should().NotContainKey("cirreum.node");
		message.SystemProperties.Should().NotContainKey("tenant");
	}

	[Fact]
	public void System_property_keys_are_compared_ordinally() {
		var message = new OutboundMessage("payload")
			.WithSystemProperty("cirreum.node", "node-1");

		message.SystemProperties.ContainsKey("Cirreum.Node").Should().BeFalse();
	}
}
