namespace Zentient.Abstractions.Testing
{
    /// <summary>
    /// Builder used to configure dependencies and mocks for a test harness.
    /// </summary>
    public interface ITestHarnessBuilder
    {
        /// <summary>
        /// Register a concrete instance for the given service type for the lifetime of the harness.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <param name="instance">Instance to register.</param>
        /// <returns>The builder for chaining.</returns>
        ITestHarnessBuilder WithDependency<T>(T instance);

        /// <summary>
        /// Configure a mock using the internal mock builder DSL and register the built mock instance as the service.
        /// </summary>
        /// <typeparam name="T">Service type to mock.</typeparam>
        /// <param name="configure">Action configuring the mock builder.</param>
        /// <returns>The builder for chaining.</returns>
        ITestHarnessBuilder WithMock<T>(Action<IMockBuilder<T>> configure);

        /// <summary>
        /// Configure a mock and expose the created verifier.
        /// </summary>
        /// <typeparam name="T">Service type to mock.</typeparam>
        /// <param name="configure">Action configuring the mock builder.</param>
        /// <param name="verifier">Out parameter that receives the verifier for the created mock.</param>
        /// <returns>The builder for chaining.</returns>
        ITestHarnessBuilder WithMock<T>(Action<IMockBuilder<T>> configure, out IMockVerifier<T> verifier);

        /// <summary>
        /// Replace an existing registration for the provided service type with the supplied instance.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <param name="instance">New instance to register.</param>
        /// <returns>The builder for chaining.</returns>
        ITestHarnessBuilder Replace<T>(T instance);

        /// <summary>
        /// Build the harness and return an <see cref="ITestHarness"/> that can resolve services.
        /// </summary>
        /// <returns>The built harness.</returns>
        ITestHarness Build();
    }
}
