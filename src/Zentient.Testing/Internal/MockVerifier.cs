using System;
using System.Linq;

using Zentient.Abstractions.Testing;

namespace Zentient.Testing.Internal
{
    /// <summary>
    /// Default verifier implementation.
    /// </summary>
    /// <typeparam name="T">The mocked interface type.</typeparam>
    internal sealed class MockVerifier<T> : IMockVerifier<T>
    {
        private readonly MockEngine _engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockVerifier{T}"/> class.
        /// </summary>
        /// <param name="engine">The mock engine to verify against.</param>
        public MockVerifier(MockEngine engine)
        {
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
        }

        /// <inheritdoc />
        public void ShouldHaveBeenCalled(string methodName)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException("Method name must be provided", nameof(methodName));

            if (!_engine.Calls.Any(c => c.Method.Name == methodName))
            {
                throw new InvalidOperationException($"Expected method {methodName} to be called");
            }
        }

        /// <inheritdoc />
        public void ShouldHaveBeenCalledTimes(string methodName, int expected)
        {
            int count = _engine.Calls.Count(c => c.Method.Name == methodName);
            if (count != expected)
            {
                throw new InvalidOperationException($"Expected {methodName} to be called {expected} times but was {count}");
            }
        }
    }
}
