namespace Zentient.Abstractions.Testing
{
    /// <summary>
    /// Represents a lightweight per-test service resolver used by the harness.
    /// </summary>
    public interface ITestHarness
    {
        /// <summary>
        /// Resolve a registered service or construct one using registered dependencies.
        /// </summary>
        /// <typeparam name="T">Service type to resolve.</typeparam>
        /// <returns>Instance of the requested service.</returns>
        T Resolve<T>();
    }
}
