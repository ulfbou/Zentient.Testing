using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xunit
{
    // Only define FactAttribute in this shim when xunit is not referenced.
}

namespace Zentient.Testing
{
    /// <summary>
    /// Minimal replacement for xUnit's Assert used when xUnit is not available.
    /// </summary>
    public static class Assert
    {
        /// <summary>
        /// Assert that two values are equal using EqualityComparer&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T">Type of the values.</typeparam>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        public static void Equal<T>(T expected, T actual)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                throw new InvalidOperationException($"Assert.Equal failed. Expected: {expected}. Actual: {actual}.");
        }
    }
}
