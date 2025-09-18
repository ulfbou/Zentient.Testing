// <copyright file="It.cs" authors="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Zentient.Testing.Internal
{
    /// <summary>
    /// Helper used in expression trees to indicate a wildcard argument.
    /// </summary>
    public static class It
    {
        /// <summary>
        /// Matches any value of the given type when used in a Given expression.
        /// </summary>
        /// <typeparam name="T">The type to match.</typeparam>
        /// <returns>Default value for the type (placeholder only).</returns>
        [SuppressMessage("Style", "IDE0022:Use block body for method", Justification = "Expression-bodied is intentional here")]
        public static T IsAny<T>() => default!;
    }
}
