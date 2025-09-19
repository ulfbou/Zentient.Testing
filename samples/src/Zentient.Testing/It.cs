namespace Zentient.Testing
{
    /// <summary>
    /// Test helper used in expression-based mock setups. The mock builder recognizes calls to <see cref="It.IsAny{T}"/>.
    /// </summary>
    public static class It
    {
        /// <summary>
        /// Placeholder used in expressions to indicate any value of type T should match.
        /// This method is intended to be used inside expression trees only; its runtime value is ignored by the mock engine.
        /// </summary>
        public static T IsAny<T>() => default!;
    }
}
