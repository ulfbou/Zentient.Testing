using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Zentient.Abstractions.Testing
{
    /// <summary>
    /// Fluent API for building mocks.
    /// </summary>
    /// <typeparam name="T">The mocked interface type.</typeparam>
    public interface IMockBuilder<T>
    {
        /// <summary>
        /// Specify the method or property call to stub using an expression returning an object.
        /// </summary>
        /// <param name="expression">Expression identifying the call.</param>
        /// <returns>The mock builder for chaining.</returns>
        IMockBuilder<T> Given(Expression<Func<T, object>> expression);

        /// <summary>
        /// Specify the method or property call to stub using a void-returning expression.
        /// </summary>
        /// <param name="expression">Expression identifying the call.</param>
        /// <returns>The mock builder for chaining.</returns>
        IMockBuilder<T> Given(Expression<Action<T>> expression);

        /// <summary>
        /// Configure the previously specified call to return the provided result when invoked.
        /// </summary>
        /// <param name="result">Result object to return.</param>
        /// <returns>The mock builder for chaining.</returns>
        IMockBuilder<T> ThenReturns(object? result);

        /// <summary>
        /// Configure the previously specified call to throw the provided exception when invoked.
        /// </summary>
        /// <param name="ex">Exception to be thrown.</param>
        /// <returns>The mock builder for chaining.</returns>
        IMockBuilder<T> ThenThrows(Exception ex);

        /// <summary>
        /// Finalize the mock and return the proxy instance. Also returns a verifier instance.
        /// </summary>
        /// <param name="verifier">Out parameter receiving the verifier for assertions against the mock.</param>
        /// <returns>The proxied mock instance implementing <typeparamref name="T"/>.</returns>
        T Build(out IMockVerifier<T> verifier);
    }
}
