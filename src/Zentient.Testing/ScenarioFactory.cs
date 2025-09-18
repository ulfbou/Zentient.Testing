// <copyright file="ScenarioFactory.cs" authors="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using Zentient.Abstractions.Testing;

namespace Zentient.Testing
{
    /// <summary>
    /// Factory helpers for creating reusable test scenarios.
    /// </summary>
    public static class TestScenario
    {
        /// <summary>
        /// Create a new scenario instance that uses the supplied asynchronous act delegate.
        /// </summary>
        /// <typeparam name="TInput">The scenario input type.</typeparam>
        /// <typeparam name="TResult">The scenario result type.</typeparam>
        /// <param name="act">An asynchronous delegate that executes the system under test using the harness and input.</param>
        /// <returns>An <see cref="ITestScenario{TInput,TResult}"/> instance.</returns>
        public static ITestScenario<TInput, TResult> For<TInput, TResult>(Func<ITestHarness, TInput, CancellationToken, Task<TResult>> act)
        {
            ArgumentNullException.ThrowIfNull(act);
            var s = new Zentient.Testing.Internal.TestScenario<TInput, TResult>();
            s.SetAct((h, input, ct) => act(h, input, ct));
            return s;
        }

        /// <summary>
        /// Create a new scenario that resolves a handler of type <typeparamref name="THandler"/>
        /// from the harness and invokes the supplied asynchronous handler delegate.
        /// </summary>
        /// <typeparam name="THandler">Handler type to resolve from the harness.</typeparam>
        /// <typeparam name="TInput">The scenario input type.</typeparam>
        /// <typeparam name="TResult">The scenario result type.</typeparam>
        /// <param name="handler">An asynchronous delegate that executes the handler with the resolved instance and input.</param>
        /// <returns>An <see cref="ITestScenario{TInput,TResult}"/> instance.</returns>
        public static ITestScenario<TInput, TResult> ForHandler<THandler, TInput, TResult>(Func<THandler, TInput, CancellationToken, Task<TResult>> handler)
        {
            ArgumentNullException.ThrowIfNull(handler);
            var s = new Zentient.Testing.Internal.TestScenario<TInput, TResult>();
            s.SetAct(async (h, input, ct) =>
            {
                var resolved = h.Resolve<THandler>();
                return await handler(resolved, input, ct).ConfigureAwait(false);
            });
            return s;
        }

        /// <summary>
        /// Create a new scenario that resolves a handler of type <typeparamref name="THandler"/>
        /// from the harness and invokes the supplied synchronous handler delegate.
        /// </summary>
        /// <typeparam name="THandler">Handler type to resolve from the harness.</typeparam>
        /// <typeparam name="TInput">The scenario input type.</typeparam>
        /// <typeparam name="TResult">The scenario result type.</typeparam>
        /// <param name="handler">A synchronous delegate that executes the handler with the resolved instance and input.</param>
        /// <returns>An <see cref="ITestScenario{TInput,TResult}"/> instance.</returns>
        public static ITestScenario<TInput, TResult> ForHandler<THandler, TInput, TResult>(Func<THandler, TInput, TResult> handler)
        {
            ArgumentNullException.ThrowIfNull(handler);
            var s = new Zentient.Testing.Internal.TestScenario<TInput, TResult>();
            s.SetAct((h, input, ct) => Task.FromResult(handler(h.Resolve<THandler>(), input)));
            return s;
        }
    }
}
