// <copyright file="XunitShim.cs" authors="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zentient.Testing
{
    /// <summary>
    /// Minimal assertion helpers used when xUnit is not available. Intended as a very small shim
    /// for library internal usage and examples.
    /// </summary>
    public static class Assert
    {
        /// <summary>
        /// Verifies that the specified condition is true.
        /// </summary>
        /// <param name="condition">Condition that must be true.</param>
        /// <param name="message">Optional message to include in the failure exception.</param>
        public static void True(bool condition, string? message = null)
        {
            if (!condition) throw new InvalidOperationException(message ?? "Assert.True failed.");
        }

        /// <summary>
        /// Verifies that the specified condition is false.
        /// </summary>
        /// <param name="condition">Condition that must be false.</param>
        /// <param name="message">Optional message to include in the failure exception.</param>
        public static void False(bool condition, string? message = null)
        {
            if (condition) throw new InvalidOperationException(message ?? "Assert.False failed.");
        }

        /// <summary>
        /// Verifies that two values are equal using the default equality comparer.
        /// </summary>
        /// <typeparam name="T">Type of the values.</typeparam>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        public static void Equal<T>(T expected, T actual)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                throw new InvalidOperationException($"Assert.Equal failed. Expected: {expected}. Actual: {actual}.");
        }

        /// <summary>
        /// Verifies that the provided object is not null.
        /// </summary>
        /// <param name="obj">Object under test.</param>
        /// <param name="message">Optional failure message.</param>
        public static void NotNull(object? obj, string? message = null)
        {
            if (obj is null) throw new InvalidOperationException(message ?? "Assert.NotNull failed.");
        }

        /// <summary>
        /// Verifies that the provided object is null.
        /// </summary>
        /// <param name="obj">Object under test.</param>
        /// <param name="message">Optional failure message.</param>
        public static void Null(object? obj, string? message = null)
        {
            if (obj is not null) throw new InvalidOperationException(message ?? "Assert.Null failed.");
        }

        /// <summary>
        /// Verifies that the specified substring is contained within the actual string using ordinal comparison.
        /// </summary>
        /// <param name="expectedSubstring">Substring expected to be present.</param>
        /// <param name="actualString">String to search.</param>
        public static void Contains(string expectedSubstring, string actualString)
        {
            if (actualString is null) throw new InvalidOperationException("Assert.Contains failed. Actual string is null.");
            if (!actualString.Contains(expectedSubstring, StringComparison.Ordinal))
                throw new InvalidOperationException($"Assert.Contains failed. Expected to find '{expectedSubstring}' in '{actualString}'.");
        }
    }
}
