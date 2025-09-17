using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zentient.Abstractions.Testing;

namespace Zentient.Testing.Internal
{
    /// <summary>
    /// Simple test scenario implementation that supports Arrange -> Act -> Assert.
    /// </summary>
    /// <typeparam name="TInput">Input type.</typeparam>
    /// <typeparam name="TResult">Result type.</typeparam>
    internal sealed class TestScenario<TInput, TResult> : ITestScenario<TInput, TResult>
    {
        private readonly List<Action<ITestHarnessBuilder>> _arrangers = new List<Action<ITestHarnessBuilder>>();
        private Func<ITestHarness, TInput, CancellationToken, Task<TResult>>? _act;
        private TResult? _lastResult;

        /// <inheritdoc />
        public ITestScenario<TInput, TResult> Arrange(Action<ITestHarnessBuilder> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);
            _arrangers.Add(configure);
            return this;
        }

        /// <summary>
        /// Sets the Act delegate directly. This would typically be done by a factory in real usage.
        /// </summary>
        public void SetAct(Func<ITestHarness, TInput, CancellationToken, Task<TResult>> act)
        {
            _act = act ?? throw new ArgumentNullException(nameof(act));
        }

        /// <inheritdoc />
        public async Task<TResult> ActAsync(TInput input, CancellationToken ct = default)
        {
            if (_act is null)
                throw new InvalidOperationException("Act has not been configured for this scenario.");

            var builder = new TestHarnessBuilder();
            foreach (var a in _arrangers) a(builder);
            using var harness = (TestHarness)builder.Build();
            _lastResult = await _act(harness, input, ct).ConfigureAwait(false);
            return _lastResult!;
        }

        /// <inheritdoc />
        public void Assert(Action<IResultAssertions<TResult>> assertions)
        {
            ArgumentNullException.ThrowIfNull(assertions);
            if (_lastResult is null) throw new InvalidOperationException("No result available. Ensure ActAsync has been executed before asserting.");

            var ra = new ResultAssertions<TResult>(_lastResult);
            assertions(ra);
        }
    }
}
