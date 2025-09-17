using System;
using System.Reflection;

namespace Zentient.Testing.Internal
{
    /// <summary>
    /// Information about a single invocation made on a mock.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of <see cref="CallInfo"/>.
    /// </remarks>
    /// <param name="method">The invoked method.</param>
    /// <param name="args">The arguments passed to the method.</param>
    public sealed class CallInfo(MethodInfo method, object?[] args)
    {
        /// <summary>
        /// Gets the method that was invoked.
        /// </summary>
        public MethodInfo Method { get; } = method ?? throw new ArgumentNullException(nameof(method));

        /// <summary>
        /// Gets the arguments passed to the invocation.
        /// </summary>
        public object?[] Arguments { get; } = args ?? throw new ArgumentNullException(nameof(args));

        /// <summary>
        /// Gets the UTC timestamp when the invocation occurred.
        /// </summary>
        public DateTime Timestamp { get; } = DateTime.UtcNow;
    }
}
