using System;
using System.Linq.Expressions;
using Zentient.Abstractions.Testing;

namespace Zentient.Testing.Internal
{
    /// <summary>
    /// Basic result assertion helpers.
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    internal sealed class ResultAssertions<TResult> : IResultAssertions<TResult>
    {
        private readonly TResult _value;

        /// <summary>
        /// Initializes a new instance of <see cref="ResultAssertions{TResult}"/>.
        /// </summary>
        /// <param name="value">The result value under test.</param>
        public ResultAssertions(TResult value) => _value = value;

        /// <inheritdoc />
        public void NotBeNull()
        {
            if (_value is null) throw new InvalidOperationException("Expected result to not be null.");
        }

        /// <inheritdoc />
        public void HaveValue(TResult expected)
        {
            if (!Equals(_value, expected)) throw new InvalidOperationException($"Expected value {expected} but was {_value}.");
        }

        /// <inheritdoc />
        public void WithProperty<TProperty>(Expression<Func<TResult, TProperty>> selector, TProperty expected)
        {
            ArgumentNullException.ThrowIfNull(selector);
            var func = selector.Compile();
            var actual = func(_value!);
            if (!Equals(actual, expected)) throw new InvalidOperationException($"Expected property to be {expected} but was {actual}.");
        }

        /// <inheritdoc />
        public IResultAssertions<TResult> AndAlso => this;
    }
}
