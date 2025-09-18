// <copyright file="CallInfo.cs" authors="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Reflection;

namespace Zentient.Testing.Internal
{
    /// <summary>
    /// Contains information about a single invocation performed against a mock proxy.
    /// </summary>
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
