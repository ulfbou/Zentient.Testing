namespace Zentient.Abstractions.Testing
{
    /// <summary>
    /// Verifier API for asserting calls were made on mocks.
    /// </summary>
    /// <typeparam name="T">The mocked interface type.</typeparam>
    public interface IMockVerifier<T>
    {
        /// <summary>
        /// Assert that a method with the specified name was called at least once.
        /// </summary>
        /// <param name="methodName">Name of the method expected to have been called.</param>
        void ShouldHaveBeenCalled(string methodName);

        /// <summary>
        /// Assert that a method with the specified name was called the expected number of times.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="expected">Expected call count.</param>
        void ShouldHaveBeenCalledTimes(string methodName, int expected);
    }
}
