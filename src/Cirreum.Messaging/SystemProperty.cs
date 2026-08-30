namespace Cirreum.Messaging;

/// <summary>
/// The reservation rule for system property keys.
/// </summary>
/// <remarks>
/// The reservation is a rule about key shape, not a list of keys: this package owns the notion that
/// a key is reserved, and the framework component that writes a system property owns the key itself.
/// </remarks>
public static class SystemProperty {

	/// <summary>
	/// The prefix every system property key carries.
	/// </summary>
	public const string ReservedPrefix = "cirreum.";

	/// <summary>
	/// Determines whether <paramref name="key"/> is reserved for system properties.
	/// </summary>
	/// <param name="key">The property key to test.</param>
	/// <returns><see langword="true"/> when the key carries <see cref="ReservedPrefix"/>.</returns>
	public static bool IsReserved(string? key) =>
		key is not null && key.StartsWith(ReservedPrefix, StringComparison.Ordinal);

}
