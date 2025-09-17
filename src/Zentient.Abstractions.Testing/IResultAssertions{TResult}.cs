using System.Linq.Expressions;

namespace Zentient.Abstractions.Testing
{
    /// <summary>
    /// Synchronous assertion helpers used by scenarios.
    /// </summary>
    /// <typeparam name="TResult">Result type under assertion.</typeparam>
    public interface IResultAssertions<TResult>
    {
        /// <summary>
        /// Assert that the result is not null.
        /// </summary>
        void NotBeNull();

        /// <summary>
        /// Assert that the result has the expected value.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        void HaveValue(TResult expected);

        /// <summary>
        /// Assert on a selected property of the result.
        /// </summary>
        /// <typeparam name="TProperty">Property type.</typeparam>
        /// <param name="selector">Selector to pick the property from the result.</param>
        /// <param name="expected">Expected property value.</param>
        void WithProperty<TProperty>(Expression<Func<TResult, TProperty>> selector, TProperty expected);

        /// <summary>
        /// Allows chaining of assertions.
        /// </summary>
        IResultAssertions<TResult> AndAlso { get; }
    }
}
