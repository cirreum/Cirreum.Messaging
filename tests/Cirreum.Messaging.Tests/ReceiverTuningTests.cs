namespace Cirreum.Messaging.Tests;

/// <summary>
/// Proofs for <see cref="ReceiverTuning"/>: the documented bounds are enforced where the value is
/// set rather than left to each broker implementation, and equality holds, since receivers are
/// cached by name and an implementation compares tuning to decide whether a cached receiver may
/// be returned.
/// </summary>
public class ReceiverTuningTests {

	[Fact]
	public void A_negative_prefetch_count_is_rejected() {
		var construct = () => new ReceiverTuning { PrefetchCount = -1 };

		construct.Should().Throw<ArgumentOutOfRangeException>()
			.And.ParamName.Should().Be(nameof(ReceiverTuning.PrefetchCount));
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(1000)]
	public void A_non_negative_prefetch_count_is_accepted(int prefetchCount) {
		new ReceiverTuning { PrefetchCount = prefetchCount }
			.PrefetchCount.Should().Be(prefetchCount);
	}

	[Fact]
	public void An_unset_prefetch_count_is_empty() {
		var tuning = new ReceiverTuning();

		tuning.PrefetchCount.Should().BeNull();
		tuning.IsEmpty.Should().BeTrue();
	}

	[Fact]
	public void An_explicit_zero_is_not_empty() {
		// 0 disables prefetch — an explicit instruction to the broker, not an absent value.
		new ReceiverTuning { PrefetchCount = 0 }.IsEmpty.Should().BeFalse();
	}

	[Fact]
	public void Equal_tuning_compares_equal() {
		var left = new ReceiverTuning { PrefetchCount = 10 };
		var right = new ReceiverTuning { PrefetchCount = 10 };

		left.Should().Be(right);
		left.GetHashCode().Should().Be(right.GetHashCode());
	}

	[Fact]
	public void Different_tuning_compares_unequal() {
		new ReceiverTuning { PrefetchCount = 10 }
			.Should().NotBe(new ReceiverTuning { PrefetchCount = 20 });

		new ReceiverTuning { PrefetchCount = 0 }
			.Should().NotBe(new ReceiverTuning());
	}
}
