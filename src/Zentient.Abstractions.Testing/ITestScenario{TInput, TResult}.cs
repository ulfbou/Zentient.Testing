namespace Zentient.Abstractions.Testing
{
    /// <summary>
    /// Represents a test scenario for arranging, acting and asserting results.
    /// </summary>
    /// <typeparam name="TInput">Type of the input to the scenario.</typeparam>
    /// <typeparam name="TResult">Type of the result produced by the scenario.</typeparam>
    public interface ITestScenario<TInput, TResult>
    {
        /// <summary>
        /// Add an arrange action that configures the harness builder.
        /// </summary>
        /// <param name="configure">Configure action for the harness builder.</param>
        /// <returns>The scenario for chaining.</returns>
        ITestScenario<TInput, TResult> Arrange(Action<ITestHarnessBuilder> configure);

        /// <summary>
        /// Executes the scenario's act step asynchronously using the supplied input.
        /// </summary>
        /// <param name="input">The input for the scenario.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task producing the result.</returns>
        Task<TResult> ActAsync(TInput input, CancellationToken ct = default);

        /// <summary>
        /// Run synchronous assertions against the produced result.
        /// </summary>
        /// <param name="assertions">Assertions to execute against the result.</param>
        void Assert(Action<IResultAssertions<TResult>> assertions);
    }
}
