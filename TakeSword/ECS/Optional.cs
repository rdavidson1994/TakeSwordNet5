namespace TakeSword
{
    /// <summary>
    /// When used as a system parameter, indicates that null values should still be matched.
    /// The (possibly null) underlying value can be accessed via the Value property.
    /// </summary>
    /// <typeparam name="T">The underlying type of the parameter.</typeparam>
    public record Optional<T>(T? Value) : IOptional where T : class;
}